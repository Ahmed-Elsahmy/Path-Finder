using BLL.Common;
using BLL.Dtos.CourseDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BLL.Services.CourseRecommendationService
{
    public class CourseRecommendationService : ICourseRecommendationService
    {
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<UserSkill> _userSkillRepo;
        private readonly IRepository<CourseSkill> _courseSkillRepo;
        private readonly IRepository<CourseProgress> _progressRepo;
        private readonly IRepository<CV> _cvRepo;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CourseRecommendationService> _logger;
        private static int _currentGeminiKeyIndex = 0;
        private const string GeminiBaseUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        public CourseRecommendationService(
            IRepository<Course> courseRepo,
            IRepository<UserSkill> userSkillRepo,
            IRepository<CourseSkill> courseSkillRepo,
            IRepository<CourseProgress> progressRepo,
            IRepository<CV> cvRepo,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            ILogger<CourseRecommendationService> logger)
        {
            _courseRepo = courseRepo;
            _userSkillRepo = userSkillRepo;
            _courseSkillRepo = courseSkillRepo;
            _progressRepo = progressRepo;
            _cvRepo = cvRepo;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ServiceResult<CourseRecommendationListRS>> GetRecommendationsAsync(
            string userId,
            string? targetJobTitle = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Get user's current skills
                var userSkills = await _userSkillRepo.Query()
                    .Where(us => us.UserId == userId)
                    .Include(us => us.Skill)
                    .Select(us => new { us.Skill.SkillName, us.ProficiencyLevel })
                    .Distinct()
                    .ToListAsync(cancellationToken);

                // 2. Get user's recommended skills from CV (if any)
                var latestCv = await _cvRepo.Query()
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.UploadedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                var recommendedSkills = latestCv?.RecommendedSkills ?? new List<string>();
                var suggestedJobTitles = latestCv?.SuggestedJobTitles ?? new List<string>();

                // 3. Get courses the user has NOT enrolled in
                var enrolledCourseIds = await _progressRepo.Query()
                    .Where(p => p.UserId == userId)
                    .Select(p => p.CourseId)
                    .ToListAsync(cancellationToken);

                var availableCourses = await _courseRepo.Query()
                    .Include(c => c.Platform)
                    .Include(c => c.CourseSkills)
                        .ThenInclude(cs => cs.Skill)
                    .Where(c => !enrolledCourseIds.Contains(c.Id))
                    .Take(50) // Limit to prevent huge payloads
                    .ToListAsync(cancellationToken);

                if (!availableCourses.Any())
                    return ServiceResult<CourseRecommendationListRS>.Failure(
                        "No available courses found to recommend.",
                        ServiceErrorCode.NotFound);

                // 4. Build context for Gemini
                var skillNames = userSkills.Select(s => $"{s.SkillName} ({s.ProficiencyLevel ?? "Unknown"})").ToList();
                var courseDescriptions = availableCourses.Select(c => new
                {
                    c.Id,
                    c.Name,
                    Description = c.Description ?? "No description",
                    Platform = c.Platform?.Name ?? "Unknown",
                    c.DifficultyLevel,
                    c.IsFree,
                    c.Price,
                    Skills = c.CourseSkills?.Select(cs => cs.Skill?.SkillName ?? "").Distinct().ToList() ?? new List<string>()
                }).ToList();

                var targetJob = targetJobTitle
                    ?? suggestedJobTitles.FirstOrDefault()
                    ?? "general career growth";

                var prompt = $@"
You are an AI course recommendation engine. Analyze the user's profile and available courses, then recommend the TOP 5 most relevant courses.

═══════════ USER PROFILE ═══════════
Current Skills: {(skillNames.Any() ? string.Join(", ", skillNames) : "None")}
Recommended Skills to Learn (from CV analysis): {(recommendedSkills.Any() ? string.Join(", ", recommendedSkills) : "None")}
Career Target: {targetJob}

═══════════ AVAILABLE COURSES ═══════════
{JsonSerializer.Serialize(courseDescriptions, new JsonSerializerOptions { WriteIndented = true })}

═══════════ INSTRUCTIONS ═══════════
Return ONLY a valid JSON object with these exact keys:

""Recommendations"": array of up to 5 objects, each with:
  - ""CourseId"": integer (must match an Id from the available courses above)
  - ""MatchScore"": integer 0-100 (how relevant this course is for the user)
  - ""AIRecommendationReason"": string (2-3 sentences explaining WHY this course is recommended)
  - ""SkillsYouWillLearn"": array of strings (specific skills from this course the user doesn't have yet)

""OverallAdvice"": string (2-3 sentences of personalized career learning advice)

Sort by MatchScore descending (highest first).
Prioritize courses that fill skill gaps and align with the career target.
Do not recommend courses that teach skills the user already has at Advanced level.
";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                    generationConfig = new
                    {
                        temperature = 0.3,
                        responseMimeType = "application/json"
                    }
                };

                var aiResult = await CallGeminiAsync(requestBody, cancellationToken);

                if (aiResult == null)
                    return ServiceResult<CourseRecommendationListRS>.Failure(
                        "AI recommendation failed. Please try again.",
                        ServiceErrorCode.UpstreamServiceError);

                // 5. Enrich AI response with full course data
                foreach (var rec in aiResult.Recommendations)
                {
                    var course = availableCourses.FirstOrDefault(c => c.Id == rec.CourseId);
                    if (course != null)
                    {
                        rec.CourseName = course.Name;
                        rec.Description = course.Description;
                        rec.PlatformName = course.Platform?.Name;
                        rec.ThumbnailUrl = course.ThumbnailUrl;
                        rec.DifficultyLevel = course.DifficultyLevel;
                        rec.IsFree = course.IsFree;
                        rec.Price = course.Price;
                        rec.ExternalUrl = course.ExternalUrl;
                    }
                }

                // Remove any recommendations where courseId didn't match
                aiResult.Recommendations = aiResult.Recommendations
                    .Where(r => availableCourses.Any(c => c.Id == r.CourseId))
                    .ToList();

                return ServiceResult<CourseRecommendationListRS>.Success(aiResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating course recommendations for user {UserId}", userId);
                return ServiceResult<CourseRecommendationListRS>.Failure(
                    "An unexpected error occurred while generating recommendations.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }

        private async Task<CourseRecommendationListRS?> CallGeminiAsync(
            object requestBody,
            CancellationToken ct)
        {
            try
            {
                var apiKeys = GetGeminiApiKeys();

                if (!apiKeys.Any())
                {
                    _logger.LogWarning(
                        "No Gemini API keys configured.");

                    return null;
                }

                var client =
                    _httpClientFactory.CreateClient("GeminiClient");

                int totalKeys = apiKeys.Count;

                for (int i = 0; i < totalKeys; i++)
                {
                    var index =
                        Interlocked.Increment(ref _currentGeminiKeyIndex);

                    var apiKey = apiKeys[index % totalKeys];

                    try
                    {
                        client.DefaultRequestHeaders.Remove("x-goog-api-key");

                        client.DefaultRequestHeaders.TryAddWithoutValidation(
                            "x-goog-api-key",
                            apiKey);

                        var httpContent = new StringContent(
                            JsonSerializer.Serialize(requestBody),
                            Encoding.UTF8,
                            "application/json");

                        var response = await client.PostAsync(
                            GeminiBaseUrl,
                            httpContent,
                            ct);

                        var raw =
                            await response.Content.ReadAsStringAsync(ct);

                        // SUCCESS
                        if (response.IsSuccessStatusCode)
                        {
                            using var doc = JsonDocument.Parse(raw);

                            var aiText = doc.RootElement
                                .GetProperty("candidates")[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text")
                                .GetString();

                            aiText = aiText?
                                .Replace("```json", "")
                                .Replace("```", "")
                                .Trim();

                            if (string.IsNullOrWhiteSpace(aiText))
                                continue;

                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };

                            return JsonSerializer.Deserialize<CourseRecommendationListRS>(
                                aiText,
                                options);
                        }

                        // RETRYABLE ERRORS
                        if ((int)response.StatusCode == 429 ||
                            (int)response.StatusCode == 503)
                        {
                            _logger.LogWarning(
                                "Gemini key failed with {Status}. Trying next key...",
                                response.StatusCode);

                            continue;
                        }

                        // OTHER ERRORS
                        _logger.LogError(
                            "Gemini error {Status}: {Body}",
                            response.StatusCode,
                            raw);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogWarning(
                            "Gemini request timed out. Trying next key...");

                        continue;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Gemini key failed. Trying next key...");
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Gemini API call failed for course recommendations");

                return null;
            }
        }
        private List<string> GetGeminiApiKeys()
        {
            return _config
                .GetSection("Gemini:ApiKeys")
                .Get<List<string>>() ?? new List<string>();
        }
    }
}
