using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Common;
using BLL.Dtos.CareerPathCourseDtos;
using BLL.Dtos.CareerPathDtos;
using DAL.Helper.Enums;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Services.CareerPathServices
{
    public class CareerPathService : ICareerPathService
    {
        private readonly IRepository<CareerPath> _careerpathRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<CareerPathCourse> _careerPathCourseRepository;

        // FIX 1: Added repositories to validate CategoryId / SubCategoryId exist
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<SubCategory> _subCategoryRepository;

        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ILogger<CareerPathService> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string GeminiBaseUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        private const int MaxDesiredCourses = 20;
        private const int MaxCoursesForAiPrompt = 60;

        public CareerPathService(
            IRepository<CareerPath> careerpathRepository,
            IRepository<Course> courseRepository,
            IRepository<CareerPathCourse> careerPathCourseRepository,
            IRepository<Category> categoryRepository,
            IRepository<SubCategory> subCategoryRepository,
            IWebHostEnvironment env,
            IMapper mapper,
            ILogger<CareerPathService> logger,
            IConfiguration config,
            IHttpClientFactory httpClientFactory)
        {
            _careerpathRepository = careerpathRepository;
            _courseRepository = courseRepository;
            _careerPathCourseRepository = careerPathCourseRepository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _env = env;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ServiceResult<CareerPathRS>> CreateCareerPathAsync(CareerPathRQ request)
        {
            try
            {
                // ── 1. Validate CategoryId and SubCategoryId exist ─────────────────────
                // FIX 2: These checks prevent FK constraint violations (500 errors) when
                //         the caller passes IDs that don't exist in the database.
                if (request.CategoryId.HasValue)
                {
                    var categoryExists = await _categoryRepository
                        .AnyAsync(c => c.Id == request.CategoryId.Value);

                    if (!categoryExists)
                        return ServiceResult<CareerPathRS>.Failure(
                            $"Category with id {request.CategoryId.Value} was not found.",
                            ServiceErrorCode.ValidationError);
                }

                if (request.SubCategoryId.HasValue)
                {
                    // Also verify the subcategory belongs to the given category (if both are provided)
                    var subCatQuery = request.CategoryId.HasValue
                        ? await _subCategoryRepository.AnyAsync(
                            sc => sc.Id == request.SubCategoryId.Value &&
                                  sc.CategoryId == request.CategoryId.Value)
                        : await _subCategoryRepository.AnyAsync(
                            sc => sc.Id == request.SubCategoryId.Value);

                    if (!subCatQuery)
                        return ServiceResult<CareerPathRS>.Failure(
                            request.CategoryId.HasValue
                                ? $"SubCategory with id {request.SubCategoryId.Value} was not found under category {request.CategoryId.Value}."
                                : $"SubCategory with id {request.SubCategoryId.Value} was not found.",
                            ServiceErrorCode.ValidationError);
                }

                // ── 2. Duplicate check ─────────────────────────────────────────────────
                var existing = await _careerpathRepository.FirstOrDefaultAsync(x =>
                    x.PathName == request.CareerPathName &&
                    x.DifficultyLevel == request.DifficultyLevel);

                if (existing != null)
                    return ServiceResult<CareerPathRS>.Failure(
                        "A career path with the same name and difficulty already exists.",
                        ServiceErrorCode.ValidationError);

                // ── 3. Map entity (do NOT save yet) ───────────────────────────────────
                // FIX 3: We no longer call SaveChangesAsync here. All DB work is done
                //         in a single SaveChangesAsync at the end (step 9), so a failure
                //         in AI or course-relation building leaves nothing in the DB.
                var careerPath = _mapper.Map<CareerPath>(request);

                // ── 4. Load related courses ────────────────────────────────────────────
                bool hasCategoryFilter =
                    request.SubCategoryId.HasValue || request.CategoryId.HasValue;

                IQueryable<Course> relatedCoursesQuery = _courseRepository.Query()
                    .Include(c => c.Category)
                    .Include(c => c.SubCategory);

                if (request.SubCategoryId.HasValue)
                    relatedCoursesQuery = relatedCoursesQuery
                        .Where(c => c.SubCategoryId == request.SubCategoryId.Value);
                else if (request.CategoryId.HasValue)
                    relatedCoursesQuery = relatedCoursesQuery
                        .Where(c => c.CategoryId == request.CategoryId.Value);

                var relatedCourses = await relatedCoursesQuery.ToListAsync();

                if (!relatedCourses.Any())
                {
                    _logger.LogWarning(
                        "No courses found for the given category/subcategory filter. " +
                        "Falling back to all courses.");

                    relatedCourses = await _courseRepository.Query()
                        .Include(c => c.Category)
                        .Include(c => c.SubCategory)
                        .ToListAsync();

                    hasCategoryFilter = false;
                }

                var desiredCount = hasCategoryFilter
                    ? Math.Min(MaxDesiredCourses, relatedCourses.Count)
                    : Math.Min(10, relatedCourses.Count);

                // ── 5. Generate AI course plan ─────────────────────────────────────────
                // FIX 4: Build a set of VALID course IDs from the courses we actually
                //         loaded. Any ID the AI hallucinates is silently dropped, which
                //         prevents FK violations when saving CareerPathCourse relations.
                var validCourseIdSet = relatedCourses.Select(c => c.Id).ToHashSet();

                List<CourseRecommendation> aiPlan = new();
                if (desiredCount > 0)
                {
                    try
                    {
                        var rawPlan = await GenerateCoursePlanAsync(
                            careerPathName: request.CareerPathName,
                            desiredCount: desiredCount,
                            courses: relatedCourses,
                            careerPathDescription: request.Description,
                            careerPathDifficulty: request.DifficultyLevel);

                        // Keep only IDs that actually exist in our course list
                        aiPlan = rawPlan
                            .Where(x => validCourseIdSet.Contains(x.CourseId))
                            .ToList();
                    }
                    catch (Exception ex)
                    {
                        // AI failure is non-fatal — fall back to rating-based ordering below
                        _logger.LogWarning(ex,
                            "AI course plan generation failed; falling back to rating-based ordering.");
                    }
                }

                // ── 6. Merge AI plan + rating-based fallback ───────────────────────────
                var aiOrderedCourseIds = aiPlan
                    .OrderBy(x => x.Order)
                    .Select(x => x.CourseId)
                    .ToList();

                var used = aiOrderedCourseIds.ToHashSet();

                var remainingCourseIds = relatedCourses
                    .Where(c => !used.Contains(c.Id))
                    .OrderByDescending(c => c.Rating ?? 0)
                    .ThenByDescending(c => c.IsFree)
                    .Select(c => c.Id)
                    .Take(Math.Max(0, desiredCount - aiOrderedCourseIds.Count))
                    .ToList();

                var finalCourseIds = aiOrderedCourseIds
                    .Concat(remainingCourseIds)
                    .Take(desiredCount)
                    .ToList();

                // ── 7. Generate AI prerequisites & outcomes ────────────────────────────
                var aiDetails = await GenerateCareerPathDetailsAsync(
                    careerPath.PathName,
                    careerPath.Description,
                    careerPath.DifficultyLevel);

                if (aiDetails != null)
                {
                    careerPath.Prerequisites = aiDetails.Prerequisites;
                    careerPath.ExpectedOutcomes = aiDetails.ExpectedOutcomes;
                }

                // ── 8. Build CareerPathCourse relations in memory ──────────────────────
                // Relations reference careerPath by object reference so EF resolves the
                // FK automatically after AddAsync — no need for a real CareerPathId yet.
                var aiLookup = aiPlan.ToDictionary(x => x.CourseId, x => x);
                var relations = new List<CareerPathCourse>(finalCourseIds.Count);

                for (int i = 0; i < finalCourseIds.Count; i++)
                {
                    var courseId = finalCourseIds[i];
                    aiLookup.TryGetValue(courseId, out var rec);

                    relations.Add(new CareerPathCourse
                    {
                        CareerPath = careerPath,          // navigation ref — EF sets FK on save
                        CourseId = courseId,
                        OrderNumber = i + 1,
                        IsRequired = rec?.IsRequired ?? true,
                        CompletionCriteria = rec?.Reason ?? "Related course"
                    });
                }

                careerPath.TotalCourses = relations.Count;

                // ── 9. Persist everything in ONE round-trip ────────────────────────────
                // FIX 5: Single SaveChangesAsync. Because all repositories share the
                //         same AppDbContext, AddAsync + AddRangeAsync both queue changes
                //         into the same unit of work. One SaveChangesAsync commits them
                //         atomically. If anything above failed, nothing was written to DB.
                await _careerpathRepository.AddAsync(careerPath);

                if (relations.Any())
                    await _careerPathCourseRepository.AddRangeAsync(relations);

                await _careerpathRepository.SaveChangesAsync();

                // ── 10. Reload with full includes and return ───────────────────────────
                // FIX 6: AsNoTracking() forces EF to issue a fresh SQL query and build a
                //         NEW object graph instead of returning the already-tracked entity
                //         from its identity map. This guarantees Category, SubCategory, and
                //         CareerPathCourse.CareerPath are all properly populated for AutoMapper.
                var created = await _careerpathRepository.Query()
                    .AsNoTracking()
                    .Include(cp => cp.Category)
                    .Include(cp => cp.SubCategory)
                    .Include(cp => cp.CareerPathCourses)
                        .ThenInclude(cpc => cpc.Course)
                    .FirstOrDefaultAsync(cp => cp.CareerPathId == careerPath.CareerPathId);

                if (created == null)
                    return ServiceResult<CareerPathRS>.Failure(
                        "Career path was saved but could not be reloaded.",
                        ServiceErrorCode.UpstreamServiceError);

                var response = _mapper.Map<CareerPathRS>(created);

                // Overwrite Courses with ordered, individually mapped list
                response.Courses = created.CareerPathCourses
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => _mapper.Map<CareerPathCourseRS>(x))
                    .ToList();

                return ServiceResult<CareerPathRS>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating CareerPath");
                return ServiceResult<CareerPathRS>.Failure(
                    "An error occurred while creating the career path.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }

        // ── AI: Course ordering plan ───────────────────────────────────────────────────
        private async Task<List<CourseRecommendation>> GenerateCoursePlanAsync(
            string careerPathName,
            int desiredCount,
            List<Course> courses,
            string? careerPathDescription = null,
            DifficultyLevel? careerPathDifficulty = null)
        {
            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Gemini API key is missing.");

            var limitedCourses = courses.Take(MaxCoursesForAiPrompt).ToList();
            var expectedCount = Math.Min(desiredCount, limitedCourses.Count);

            var courseText = string.Join("\n", limitedCourses.Select(c =>
            {
                var category = c.Category?.Name ?? "General";
                var subCategory = c.SubCategory?.Name ?? "N/A";
                var description = c.Description ?? "N/A";

                if (description.Length > 120)
                    description = description[..120];

                return $"Id: {c.Id}, Title: {c.Name}, Category: {category}, " +
                       $"SubCategory: {subCategory}, Description: {description}";
            }));

            var prompt = $@"
You are an AI Career Path Planner.

Return ONLY valid JSON array.

Schema:
[
  {{
    ""CourseId"": number,
    ""Order"": number,
    ""IsRequired"": boolean,
    ""Reason"": string
  }}
]

Rules:
- Return exactly {expectedCount} items
- Order starts from 1
- No duplicate CourseId
- Reason max 10 words
- No markdown
- No explanation
- MUST be valid JSON

Career Path:
{careerPathName}
Description: {careerPathDescription ?? "Not provided"}
Difficulty: {careerPathDifficulty?.ToString() ?? "Not provided"}

Courses:
{courseText}
";

            var body = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var httpContent = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var client = _httpClientFactory.CreateClient("GeminiClient");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", apiKey);

            HttpResponseMessage response = null!;

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                response = await client.PostAsync(GeminiBaseUrl, httpContent);

                if (response.IsSuccessStatusCode) break;

                if ((int)response.StatusCode == 503 && attempt < 3)
                {
                    _logger.LogWarning("Gemini 503 on attempt {Attempt}, retrying...", attempt);
                    await Task.Delay(3000);
                }
                else
                {
                    throw new HttpRequestException(
                        $"Gemini API failed with status {response.StatusCode}");
                }
            }

            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini error: {responseString}");

            using var doc = JsonDocument.Parse(responseString);

            var aiText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            aiText = aiText?.Replace("```json", "").Replace("```", "").Trim();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<List<CourseRecommendation>>(aiText ?? "[]", options);

            if (result == null || !result.Any())
                throw new Exception("AI returned empty or invalid course plan.");

            return result;
        }

        // ── AI: Prerequisites & Expected Outcomes ──────────────────────────────────────
        private async Task<AiCareerPathDetails?> GenerateCareerPathDetailsAsync(
            string careerPathName,
            string? careerPathDescription,
            DifficultyLevel? careerPathDifficulty)
        {
            try
            {
                var apiKey = _config["Gemini:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogWarning("Gemini API key missing.");
                    return null;
                }

                var prompt = $@"
You are an AI Career Path Designer.

Return ONLY valid JSON object with exactly two properties:

{{
  ""Prerequisites"": string,
  ""ExpectedOutcomes"": string
}}

Rules:
- Each field max 600 characters
- No markdown
- No explanation

Career Path Name: {careerPathName}
Description: {careerPathDescription ?? "Not provided"}
Difficulty: {careerPathDifficulty?.ToString() ?? "Not provided"}
";

                var body = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                var httpContent = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                var client = _httpClientFactory.CreateClient("GeminiClient");
                client.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", apiKey);

                var response = await client.PostAsync(GeminiBaseUrl, httpContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini details error: {Body}", responseString);
                    return null;
                }

                using var doc = JsonDocument.Parse(responseString);

                var aiText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                aiText = aiText?.Replace("```json", "").Replace("```", "").Trim();

                return JsonSerializer.Deserialize<AiCareerPathDetails>(
                    aiText ?? "{}",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate career path details.");
                return null;
            }
        }

        // ── CRUD ───────────────────────────────────────────────────────────────────────
        public async Task<ServiceResult<string>> DeleteCareerPathAsync(int id)
        {
            try
            {
                var careerPath = await _careerpathRepository.GetByIdAsync(id);
                if (careerPath == null)
                    return ServiceResult<string>.Failure(
                        "Career path not found.", ServiceErrorCode.NotFound);

                _careerpathRepository.Remove(careerPath);
                await _careerpathRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Career path deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CareerPath {Id}", id);
                return ServiceResult<string>.Failure(
                    "An error occurred while deleting the career path.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<List<CareerPathRS>>> GetAllCareerPathsAsync()
        {
            try
            {
                var careerPaths = await _careerpathRepository.Query()
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                    .Include(x => x.CareerPathCourses)
                        .ThenInclude(x => x.Course)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var result = _mapper.Map<List<CareerPathRS>>(careerPaths);
                return ServiceResult<List<CareerPathRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CareerPaths");
                return ServiceResult<List<CareerPathRS>>.Failure(
                    "Error retrieving career paths.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<CareerPathRS>> GetCareerPathByIdAsync(int id)
        {
            try
            {
                var careerPath = await _careerpathRepository.Query()
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                    .Include(x => x.CareerPathCourses)
                        .ThenInclude(x => x.Course)
                    .FirstOrDefaultAsync(c => c.CareerPathId == id);

                if (careerPath == null)
                    return ServiceResult<CareerPathRS>.Failure(
                        "Career path not found.", ServiceErrorCode.NotFound);

                var result = _mapper.Map<CareerPathRS>(careerPath);
                result.Courses = careerPath.CareerPathCourses
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => _mapper.Map<CareerPathCourseRS>(x))
                    .ToList();

                return ServiceResult<CareerPathRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CareerPath {Id}", id);
                return ServiceResult<CareerPathRS>.Failure(
                    $"Error retrieving career path with id {id}.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<CareerPathRS>> UpdateCareerPathAsync(
            int id, UpdateCareerPathRQ request)
        {
            try
            {
                var careerPath = await _careerpathRepository.GetByIdAsync(id);
                if (careerPath == null)
                    return ServiceResult<CareerPathRS>.Failure(
                        "Career path not found.", ServiceErrorCode.NotFound);

                _mapper.Map(request, careerPath);
                await _careerpathRepository.SaveChangesAsync();

                var updated = await _careerpathRepository.Query()
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                    .Include(x => x.CareerPathCourses)
                        .ThenInclude(x => x.Course)
                    .FirstOrDefaultAsync(c => c.CareerPathId == id);

                if (updated == null)
                    return ServiceResult<CareerPathRS>.Failure(
                        "Career path updated but could not be loaded.",
                        ServiceErrorCode.UpstreamServiceError);

                var result = _mapper.Map<CareerPathRS>(updated);
                result.Courses = updated.CareerPathCourses
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => _mapper.Map<CareerPathCourseRS>(x))
                    .ToList();

                return ServiceResult<CareerPathRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating CareerPath {Id}", id);
                return ServiceResult<CareerPathRS>.Failure(
                    "An error occurred while updating the career path.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }
    }
}