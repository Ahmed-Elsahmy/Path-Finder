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

        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ILogger<CareerPathService> _logger;

        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string GeminiBaseUrl =
         "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        public CareerPathService(
          IRepository<CareerPath> careerpathRepository,
          IRepository<Course> courseRepository,
          IRepository<CareerPathCourse> careerPathCourseRepository,
          IWebHostEnvironment env,
          IMapper mapper,
          ILogger<CareerPathService> logger,
          IConfiguration config,
          IHttpClientFactory httpClientFactory)
        {
            _careerpathRepository = careerpathRepository;
            _courseRepository = courseRepository;
            _careerPathCourseRepository = careerPathCourseRepository;

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

                var existing = await _careerpathRepository.FirstOrDefaultAsync(x =>
                    x.PathName == request.CareerPathName &&
                    x.DifficultyLevel == request.DifficultyLevel);

                if (existing != null)
                    return ServiceResult<CareerPathRS>.Failure("Career path already exists", ServiceErrorCode.ValidationError);

                var careerPath = _mapper.Map<CareerPath>(request);

                await _careerpathRepository.AddAsync(careerPath);
                await _careerpathRepository.SaveChangesAsync();

                // 🔥 Load related courses for this path (by SubCategoryId / CategoryId if provided)
                IQueryable<Course> relatedCoursesQuery = _courseRepository.Query()
                    .Include(c => c.Category)
                    .Include(c => c.SubCategory);

                if (request.SubCategoryId.HasValue)
                    relatedCoursesQuery = relatedCoursesQuery.Where(c => c.SubCategoryId == request.SubCategoryId.Value);
                else if (request.CategoryId.HasValue)
                    relatedCoursesQuery = relatedCoursesQuery.Where(c => c.CategoryId == request.CategoryId.Value);

                var relatedCourses = await relatedCoursesQuery.ToListAsync();

                if (!relatedCourses.Any())
                {
                    relatedCourses = await _courseRepository.Query()
                        .Include(c => c.Category)
                        .Include(c => c.SubCategory)
                        .ToListAsync();
                }

                // How many courses should be attached to this career path
                var hasCategoryFilter = request.SubCategoryId.HasValue || request.CategoryId.HasValue;

                var desiredCount = 
                    hasCategoryFilter
                        ? relatedCourses.Count // default = all related courses (within the chosen category/subcategory)
                        : Math.Min(10, relatedCourses.Count); // safety default when no category filter is provided

                // 🔥 Generate AI plan (selection + order)
                var aiPlan = desiredCount > 0
                    ? await GenerateCoursePlanAsync(
                        careerPathName: request.CareerPathName,
                        desiredCount: desiredCount,
                        courses: relatedCourses,
                        careerPathDescription: request.Description,
                        careerPathDifficulty: request.DifficultyLevel)
                    : new List<CourseRecommendation>();

                // ✅ Apply AI plan + fill missing courses (fallback) to reach desiredCount
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

                var aiLookup = aiPlan.ToDictionary(x => x.CourseId, x => x);
                var relations = new List<CareerPathCourse>(finalCourseIds.Count);

                for (int i = 0; i < finalCourseIds.Count; i++)
                {
                    var courseId = finalCourseIds[i];
                    aiLookup.TryGetValue(courseId, out var rec);

                    relations.Add(new CareerPathCourse
                    {
                        CareerPathId = careerPath.CareerPathId,
                        CourseId = courseId,
                        OrderNumber = i + 1,
                        IsRequired = rec?.IsRequired ?? true,
                        CompletionCriteria = rec?.Reason ?? "Related course"
                    });
                }
                var aiDetails = await GenerateCareerPathDetailsAsync(
    careerPath.PathName,
    careerPath.Description,
    careerPath.DifficultyLevel);

                if (aiDetails != null)
                {
                    careerPath.Prerequisites = aiDetails.Prerequisites;
                    careerPath.ExpectedOutcomes = aiDetails.ExpectedOutcomes;
                    await _careerPathCourseRepository.SaveChangesAsync();
                }
                if (relations.Any())
                {
                    await _careerPathCourseRepository.AddRangeAsync(relations);
                    careerPath.TotalCourses = relations.Count;
                    await _careerPathCourseRepository.SaveChangesAsync();
                }
                else
                {
                    careerPath.TotalCourses = 0;
                    await _careerpathRepository.SaveChangesAsync();
                }

                // ✅ Return created path with its courses
                var created = await _careerpathRepository.Query()
                    .Include(cp => cp.Category)
                    .Include(cp => cp.SubCategory)
                    .Include(cp => cp.CareerPathCourses)
                        .ThenInclude(cpc => cpc.Course)
                    .FirstOrDefaultAsync(cp => cp.CareerPathId == careerPath.CareerPathId);

                if (created == null)
                    return ServiceResult<CareerPathRS>.Failure("Career path created but could not be loaded.", ServiceErrorCode.UpstreamServiceError);

                var response = _mapper.Map<CareerPathRS>(created);
                response.Courses = created.CareerPathCourses
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => _mapper.Map<CareerPathCourseRS>(x))
                    .ToList();

                return ServiceResult<CareerPathRS>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating CareerPath");
                return ServiceResult<CareerPathRS>.Failure("An error occurred while creating the career path.", ServiceErrorCode.UpstreamServiceError);
            }
        }

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

            var limitedCourses = courses.Take(60).ToList();
            var expectedCount = Math.Min(desiredCount, limitedCourses.Count);

            var courseText = string.Join("\n", limitedCourses.Select(c =>
            {
                var category = c.Category?.Name ?? "General";
                var subCategory = c.SubCategory?.Name ?? "N/A";
                var description = c.Description ?? "N/A";

                if (description.Length > 120)
                    description = description.Substring(0, 120);

                return $"Id: {c.Id}, Title: {c.Name}, Category: {category}, SubCategory: {subCategory}, Description: {description}";
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
                    throw new HttpRequestException($"Gemini API failed with status {response.StatusCode}");
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

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<List<CourseRecommendation>>(aiText ?? "[]", options);

            if (result == null || !result.Any())
                throw new Exception("AI returned empty or invalid course plan.");

            return result;
        }
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
                    _logger.LogWarning("Gemini API key missing");
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
                    _logger.LogError("Gemini error: {Body}", responseString);
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

                var result = JsonSerializer.Deserialize<AiCareerPathDetails>(
                    aiText ?? "{}",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate career path details");
                return null;
            }
        }
        public async Task<ServiceResult<string>> DeleteCareerPathAsync(int id)
        {

            try
            {
                var careerpath = await _careerpathRepository.GetByIdAsync(id);
                if (careerpath == null)
                {
                    return ServiceResult<string>.Failure("Career path not found.", ServiceErrorCode.NotFound);
                }
                _careerpathRepository.Remove(careerpath);
                await _careerpathRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Career path deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CareerpPath {CareerpPath}", id);

                return ServiceResult<string>
                    .Failure("An error occurred while deleting the career path.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<List<CareerPathRS>>> GetAllCareerPathsAsync()
        {
            try
            {
                var careerpaths = await _careerpathRepository.Query()
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                        .Include(x => x.CareerPathCourses)
                        .ThenInclude(x => x.Course)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var result = _mapper.Map<List<CareerPathRS>>(careerpaths);
                return ServiceResult<List<CareerPathRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CarrerPaths");
                return ServiceResult<List<CareerPathRS>>.Failure("Error retrieving career paths.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<CareerPathRS>> GetCareerPathByIdAsync(int id)
        {
            try
            {
                var careerpath = await _careerpathRepository.Query()
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                    .Include(x => x.CareerPathCourses)
                        .ThenInclude(x => x.Course)
                    .FirstOrDefaultAsync(c => c.CareerPathId == id);

                if (careerpath == null)
                    return ServiceResult<CareerPathRS>.Failure("Career path not found.", ServiceErrorCode.NotFound);

                var result = _mapper.Map<CareerPathRS>(careerpath);
                result.Courses = careerpath.CareerPathCourses
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => _mapper.Map<CareerPathCourseRS>(x))
                    .ToList();

                return ServiceResult<CareerPathRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving CarrerPath with id {id}");
                return ServiceResult<CareerPathRS>.Failure($"Error retrieving career path with id {id}.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<CareerPathRS>> UpdateCareerPathAsync(int id, UpdateCareerPathRQ request)
        {
            try
            {
                var careerPath = await _careerpathRepository.GetByIdAsync(id);
                if (careerPath == null)
                {
                    return ServiceResult<CareerPathRS>.Failure("Career path not found.", ServiceErrorCode.NotFound);
                }

                _mapper.Map(request, careerPath);

                await _careerpathRepository.SaveChangesAsync();

                var updated = await _careerpathRepository.Query()
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                    .Include(x => x.CareerPathCourses)
                        .ThenInclude(x => x.Course)
                    .FirstOrDefaultAsync(c => c.CareerPathId == id);

                if (updated == null)
                    return ServiceResult<CareerPathRS>.Failure("Career path updated but could not be loaded.", ServiceErrorCode.UpstreamServiceError);

                var result = _mapper.Map<CareerPathRS>(updated);
                result.Courses = updated.CareerPathCourses
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => _mapper.Map<CareerPathCourseRS>(x))
                    .ToList();

                return ServiceResult<CareerPathRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating CareerPath {CareerPathID}", id);

                return ServiceResult<CareerPathRS>
                    .Failure("An error occurred while updating the CareerPath.", ServiceErrorCode.UpstreamServiceError);
            }

        }
    }
}
