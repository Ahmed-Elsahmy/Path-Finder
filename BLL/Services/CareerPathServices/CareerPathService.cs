using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Common;
using BLL.Dtos.CareerPathDtos;
using BLL.Dtos.UserProfileDtos;
using DAL.Helper.Enums;
using DAL.Models;
using DAL.Repository;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
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
        public async Task<ServiceResult<string>> CreateCareerPathAsync(CareerPathRQ request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CarrerPathName))
                    return ServiceResult<string>.Failure("Path name is required");

                var existing = await _careerpathRepository
                    .FirstOrDefaultAsync(x =>
                        x.PathName == request.CarrerPathName &&
                        x.DifficultyLevel == request.DifficultyLevel);

                if (existing != null)
                    return ServiceResult<string>.Failure("Career path already exists");

                var careerPath = _mapper.Map<CareerPath>(request);

                await _careerpathRepository.AddAsync(careerPath);
                await _careerpathRepository.SaveChangesAsync();

                var courses = (await _courseRepository.GetAllAsync()).ToList();

                // 🔥 NEW AI PLAN
                var aiPlan = await GenerateCoursePlanAsync(
                    request.CarrerPathName,
                    courses);

                // ✅ VALIDATION
                aiPlan = aiPlan
                    .Where(x => courses.Any(c => c.Id == x.CourseId))
                    .GroupBy(x => x.CourseId)
                    .Select(g => g.First())
                    .OrderBy(x => x.Order)
                    .ToList();

                // 🔁 FALLBACK if AI fails
                if (!aiPlan.Any())
                {
                    var fallbackCourses = courses
                        .Where(c => c.Category.Name.Contains(request.CarrerPathName))
                        .Take(5)
                        .ToList();

                    int order = 1;

                    aiPlan = fallbackCourses.Select(c => new CourseRecommendation
                    {
                        CourseId = c.Id,
                        Order = order++,
                        IsRequired = true,
                        Reason = "Fallback selection based on category"
                    }).ToList();
                }

                // 💾 SAVE WITH ORDER + IMPORTANCE
                var relations = aiPlan.Select(x => new CareerPathCourse
                {
                    CareerPathId = careerPath.CareerPathId,
                    CourseId = x.CourseId,
                    OrderNumber = x.Order,
                    IsRequired = x.IsRequired,
                    CompletionCriteria = x.Reason
                }).ToList();

                await _careerPathCourseRepository.AddRangeAsync(relations);
                await _careerPathCourseRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Career path created with AI learning plan 🚀");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating CareerPath");
                return ServiceResult<string>.Failure($"Error: {ex.Message}");
            }
        }

        private async Task<List<CourseRecommendation>> GenerateCoursePlanAsync(
            string careerPathName,
            List<Course> courses)
        {
            try
            {
                var apiKey = _config["Gemini:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogWarning("Gemini API key missing");
                    return BuildFallback(courses);
                }

                // 🔥 LIMIT courses (IMPORTANT)
                var limitedCourses = courses.Take(30).ToList();

                var courseText = string.Join("\n", limitedCourses.Select(c =>
                {
                    var category = c.Category?.Name ?? "General";
                    return $"Id: {c.Id}, Title: {c.Name}, Category: {category}";
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
- Max 5 items
- Order starts from 1
- No duplicate CourseId
- Reason max 10 words
- No markdown
- No explanation
- MUST be valid JSON

Career Path:
{careerPathName}

Courses:
{courseText}
";

                var body = new
                {
                    contents = new[]
                    {
                new { parts = new[] { new { text = prompt } } }
            },
                    generationConfig = new
                    {
                        temperature = 0.2,
                        topP = 0.9,
                        maxOutputTokens = 1000
                    }
                };

                var client = _httpClientFactory.CreateClient("GeminiClient");
                client.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", apiKey);

                var httpContent = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                // 🔁 RETRY LOOP
                for (int attempt = 1; attempt <= 3; attempt++)
                {
                    var response = await client.PostAsync(GeminiBaseUrl, httpContent);
                    var responseString = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation("Gemini RAW: {Response}", responseString);

                    if (!response.IsSuccessStatusCode)
                        continue;

                    using var doc = JsonDocument.Parse(responseString);

                    var aiText = ExtractAiText(doc);

                    if (string.IsNullOrWhiteSpace(aiText))
                        continue;

                    // 🧹 CLEAN
                    aiText = aiText
                        .Replace("```json", "")
                        .Replace("```", "")
                        .Trim();

                    var json = ExtractJson(aiText);

                    if (string.IsNullOrWhiteSpace(json))
                        continue;

                    List<CourseRecommendation>? parsed = null;

                    try
                    {
                        parsed = JsonSerializer.Deserialize<List<CourseRecommendation>>(json,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "JSON parsing failed");
                        continue;
                    }

                    var validated = ValidateAiResult(parsed, courses);

                    if (validated.Any())
                        return validated;

                    await Task.Delay(1000);
                }

                _logger.LogWarning("AI failed → fallback used");
                return BuildFallback(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI course planning failed");
                return BuildFallback(courses);
            }
        }
        private string ExtractAiText(JsonDocument doc)
        {
            try
            {
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0)
                {
                    var content = candidates[0].GetProperty("content");

                    if (content.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        return parts[0].GetProperty("text").GetString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract AI text");
            }

            return string.Empty;
        }
        private string ExtractJson(string input)
        {
            var start = input.IndexOf("[");
            var end = input.LastIndexOf("]");

            if (start == -1 || end == -1 || end <= start)
                return string.Empty;

            return input.Substring(start, end - start + 1);
        }
        private List<CourseRecommendation> ValidateAiResult(
    List<CourseRecommendation>? result,
    List<Course> courses)
        {
            if (result == null || !result.Any())
                return new List<CourseRecommendation>();

            var validIds = courses.Select(c => c.Id).ToHashSet();

            var cleaned = result
                .Where(x =>
                    x.CourseId > 0 &&
                    validIds.Contains(x.CourseId) &&
                    x.Order > 0)
                .GroupBy(x => x.CourseId)
                .Select(g => g.First())
                .OrderBy(x => x.Order)
                .Take(5)
                .ToList();

            // 🔥 Fix order + defaults
            for (int i = 0; i < cleaned.Count; i++)
            {
                cleaned[i].Order = i + 1;

                if (string.IsNullOrWhiteSpace(cleaned[i].Reason))
                    cleaned[i].Reason = "Recommended course";
            }

            return cleaned;
        }
        private List<CourseRecommendation> BuildFallback(List<Course> courses)
        {
            return courses
                .Take(5)
                .Select((c, i) => new CourseRecommendation
                {
                    CourseId = c.Id,
                    Order = i + 1,
                    IsRequired = true,
                    Reason = "Fallback recommendation"
                })
                .ToList();
        }
        public async Task<ServiceResult<string>> DeleteCareerPathAsync(int id)
        {

            try
            {
                var careerpath = await _careerpathRepository.GetByIdAsync(id);
                if (careerpath == null)
                {
                    return ServiceResult<string>.Failure("CareerpPath not found.");
                }
                _careerpathRepository.Remove(careerpath);
                await _careerpathRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("CareerpPath deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CareerpPath {CareerpPath}", id);

                return ServiceResult<string>
                    .Failure("An error occurred while deleting the CareerpPath.");
            }
        }

        public async Task<ServiceResult<List<CareerPathRS>>> GetAllCareerPathsAsync()
        {
            try
            {
                var careerpaths = await _careerpathRepository.GetAllAsync();
                var result = _mapper.Map<List<CareerPathRS>>(careerpaths);
                return ServiceResult<List<CareerPathRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CarrerPaths");
                return ServiceResult<List<CareerPathRS>>.Failure("Error retrieving CarrerPaths.");
            }
        }

        public async Task<ServiceResult<CareerPathRS>> GetCareerPathByIdAsync(int id)
        {
            try
            {
                var careerpath = await _careerpathRepository.FirstOrDefaultAsync(c => c.CareerPathId == id);
                var result = _mapper.Map<CareerPathRS>(careerpath);
                return ServiceResult<CareerPathRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving CarrerPath with id {id}");
                return ServiceResult<CareerPathRS>.Failure($"Error retrieving CarrerPath with id {id}.");
            }
        }

        public async Task<ServiceResult<string>> UpdateCareerPathAsync(int id, UpdateCareerPathRQ request)
        {
            try
            {
                var experience = await _careerpathRepository
                    .GetByIdAsync(id);
                if (experience == null)
                {
                    return ServiceResult<string>.Failure("CareerPath not found.");
                }
                _mapper.Map(request, experience);

                await _careerpathRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("CareerPath updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating CareerPath {CareerPathID}", id);

                return ServiceResult<string>
                    .Failure("An error occurred while updating the CareerPath.");
            }

        }
    }
}
