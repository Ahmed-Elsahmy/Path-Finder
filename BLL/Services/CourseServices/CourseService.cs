using AutoMapper;
using BLL.Common;
using BLL.Dtos.CourseDtos;
using BLL.Services.NotificationServices;
using DAL.Helper.Enums;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BLL.Services.CourseService
{
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<CoursePlatform> _platformRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<SubCategory> _subCategoryRepo;
        private readonly IRepository<CourseProgress> _progressRepo;
        private readonly IRepository<CareerPathCourse> _careerPathCourseRepo;
        private readonly IRepository<UserCareerPath> _userCareerPathRepo;
        private readonly  INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseService> _logger;

        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;

        private const string GeminiBaseUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        public CourseService(
            IRepository<Course> courseRepo,
            IRepository<CoursePlatform> platformRepo,
            IRepository<Category> categoryRepo,
            IRepository<SubCategory> subCategoryRepo,
            IRepository<CourseProgress> progressRepo,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<CourseService> logger,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            IServiceScopeFactory scopeFactory)
        {
            _courseRepo = courseRepo;
            _platformRepo = platformRepo;
            _categoryRepo = categoryRepo;
            _subCategoryRepo = subCategoryRepo;
            _progressRepo = progressRepo;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _scopeFactory = scopeFactory;
        }

        // ====================================================
        // 1. Get Courses with Smart Filtering
        // ====================================================
        public async Task<ServiceResult<List<CourseRS>>> GetCoursesAsync(CourseFilterRQ filter)
        {
            try
            {
                var query = _courseRepo.Query()
                    .Include(c => c.Platform)
                    .Include(c => c.Category)
                    .Include(c => c.SubCategory)
                    .Include(c => c.CourseSkills)
                        .ThenInclude(cs => cs.Skill)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var search = filter.SearchTerm.ToLower();
                    query = query.Where(c => c.Name.ToLower().Contains(search) ||
                                            (c.Instructor != null && c.Instructor.ToLower().Contains(search)));
                }

                if (filter.CategoryId.HasValue)
                    query = query.Where(c => c.CategoryId == filter.CategoryId.Value);

                if (filter.SubCategoryId.HasValue)
                    query = query.Where(c => c.SubCategoryId == filter.SubCategoryId.Value);

                if (filter.PlatformId.HasValue)
                    query = query.Where(c => c.PlatformId == filter.PlatformId.Value);

                if (filter.IsFreeOnly.HasValue && filter.IsFreeOnly.Value)
                    query = query.Where(c => c.IsFree);

                if (!string.IsNullOrWhiteSpace(filter.DifficultyLevel))
                    query = query.Where(c => c.DifficultyLevel == filter.DifficultyLevel);

                var courses = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
                var result = _mapper.Map<List<CourseRS>>(courses);

                return ServiceResult<List<CourseRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses");
                return ServiceResult<List<CourseRS>>.Failure("An error occurred while fetching courses.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        // ====================================================
        // 2. Get Course By ID
        // ====================================================
        public async Task<ServiceResult<CourseRS>> GetCourseByIdAsync(int id)
        {
            try
            {
                var course = await _courseRepo.Query()
                    .Include(c => c.Platform)
                    .Include(c => c.Category)
                    .Include(c => c.SubCategory)
                    .Include(c => c.CourseSkills)
                        .ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (course == null)
                    return ServiceResult<CourseRS>.Failure("Course not found.", ServiceErrorCode.NotFound);

                return ServiceResult<CourseRS>.Success(_mapper.Map<CourseRS>(course));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching course {Id}", id);
                return ServiceResult<CourseRS>.Failure("An unexpected error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }


        public async Task<ServiceResult<string>> CreateCourseAsync(CourseRQ request)
        {
            try
            {
                var platformExists = await _platformRepo.AnyAsync(p => p.Id == request.PlatformId);
                if (!platformExists)
                    return ServiceResult<string>.Failure("Selected course platform does not exist.", ServiceErrorCode.ValidationError);

                if (request.CategoryId.HasValue && request.SubCategoryId.HasValue)
                {
                    var isValidSubCat = await _subCategoryRepo.AnyAsync(sc =>
                        sc.Id == request.SubCategoryId.Value &&
                        sc.CategoryId == request.CategoryId.Value);

                    if (!isValidSubCat)
                        return ServiceResult<string>.Failure("The selected SubCategory does not belong to the selected Category.", ServiceErrorCode.ValidationError);
                }

                var course = _mapper.Map<Course>(request);
                course.CreatedAt = DateTime.UtcNow;
                course.IsFree = (course.Price == null || course.Price == 0m);

                await _courseRepo.AddAsync(course);
                await _courseRepo.SaveChangesAsync();

                await ExtractAndAssignSkillsAsync(course.Id, course.Name, course.Description);

                // 🔥 NEW: notify users (if course already linked to career paths)
                await NotifyCareerPathUsersAboutNewCourseAsync(course.Id, course.Name);

                return ServiceResult<string>.Success("Course created successfully with skills assigned.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return ServiceResult<string>.Failure("An error occurred while creating the course.", ServiceErrorCode.UpstreamServiceError);
            }
        }
        private async Task NotifyCareerPathUsersAboutNewCourseAsync(int courseId, string courseName)
        {
            try
            {
                // 🔥 get career paths that include this course
                var relatedPathIds = await _careerPathCourseRepo.Query()
                    .Where(x => x.CourseId == courseId)
                    .Select(x => x.CareerPathId)
                    .Distinct()
                    .ToListAsync();

                if (!relatedPathIds.Any())
                    return;

                // 🔥 get distinct users (avoid duplicate notifications)
                var userIds = await _userCareerPathRepo.Query()
                    .Where(x =>
                        relatedPathIds.Contains(x.CareerPathId) &&
                        x.Status != CareerPathStatus.Cancelled)
                    .Select(x => x.UserId)
                    .Distinct()
                    .ToListAsync();

                if (!userIds.Any())
                    return;

                foreach (var userId in userIds)
                {
                    try
                    {
                        var title = $"New course added to your career path 🚀";
                        var message = $"A new course \"{courseName}\" was added to your career path.";

                        await _notificationService.CreateForUserAsync(
                            userId,
                            "Update",
                            title,
                            message,
                            "Course",
                            courseId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Failed to notify user {UserId} about new course {CourseId}",
                            userId,
                            courseId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error notifying users about new course {CourseId}",
                    courseId);
            }
        }
        private async Task ExtractAndAssignSkillsAsync(int courseId, string courseName, string? courseDescription)
        {
            try
            {
                var apiKey = _config["Gemini:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogWarning("Gemini API key is missing. Skill extraction aborted.");
                    return;
                }

                _logger.LogInformation("Starting skill extraction for CourseId: {CourseId}", courseId);

                var prompt = $@"
Analyze the following course name and description. Return ONLY a JSON array of objects representing the professional and technical skills this course teaches.
Each object MUST have exactly two string properties:
1. ""SkillName"": The name of the skill.
2. ""SkillLevel"": Determine the level of this skill based on the course description (ONLY use one of these words: 'Beginner', 'Intermediate', or 'Advanced').
Do not use markdown formatting or ```json.
Course Name: {courseName}
Description: {courseDescription}";

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
                        _logger.LogWarning("Gemini 503 on attempt {Attempt}, retrying in 3s...", attempt);
                        await Task.Delay(3000);
                    }
                    else break;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Gemini status: {Status}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini error response: {Body}", responseString);
                    return;
                }

                using var doc = JsonDocument.Parse(responseString);
                var aiText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                aiText = aiText?.Replace("```json", "").Replace("```", "").Trim();
                _logger.LogInformation("AI extracted text: {Text}", aiText);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var extractedSkills = JsonSerializer.Deserialize<List<AiExtractedSkill>>(aiText ?? "[]", options);

                _logger.LogInformation("Skills parsed count: {Count}", extractedSkills?.Count ?? 0);

                if (extractedSkills == null || !extractedSkills.Any())
                {
                    _logger.LogWarning("No skills extracted from AI response.");
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var skillRepo = scope.ServiceProvider.GetRequiredService<IRepository<Skill>>();
                var courseSkillRepo = scope.ServiceProvider.GetRequiredService<IRepository<CourseSkill>>();

                foreach (var item in extractedSkills)
                {
                    if (string.IsNullOrWhiteSpace(item.SkillName)) continue;

                    var safeName = item.SkillName.Length > 200
                        ? item.SkillName.Substring(0, 200)
                        : item.SkillName;

                    var safeLevel = string.IsNullOrWhiteSpace(item.SkillLevel)
                        ? "Beginner"
                        : item.SkillLevel;

                    // Find or create the skill globally
                    var globalSkill = await skillRepo.Query()
                        .FirstOrDefaultAsync(s => s.SkillName.ToLower() == safeName.ToLower());

                    if (globalSkill == null)
                    {
                        globalSkill = new Skill
                        {
                            SkillName = safeName,
                            Category = "Course Extracted",
                            IsTechnical = true
                        };
                        await skillRepo.AddAsync(globalSkill);
                        await skillRepo.SaveChangesAsync();
                        _logger.LogInformation("New skill created: {Name}", safeName);
                    }

                    // Link skill to course if not already linked
                    var alreadyLinked = await courseSkillRepo.AnyAsync(cs =>
                        cs.CourseId == courseId && cs.SkillId == globalSkill.SkillId);

                    if (!alreadyLinked)
                    {
                        await courseSkillRepo.AddAsync(new CourseSkill
                        {
                            CourseId = courseId,
                            SkillId = globalSkill.SkillId,
                            SkillLevel = safeLevel
                        });
                        _logger.LogInformation("Skill linked: {Name} ({Level}) to CourseId {CourseId}", safeName, safeLevel, courseId);
                    }
                }

                await courseSkillRepo.SaveChangesAsync();
                _logger.LogInformation("All skills saved successfully for CourseId: {CourseId}", courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Skill extraction failed for CourseId: {CourseId}", courseId);
            }
        }
        public async Task<ServiceResult<string>> UpdateCourseAsync(int id, UpdateCourseRQ request, IFormCollection form)
        {
            try
            {
                var course = await _courseRepo.GetByIdAsync(id);
                if (course == null)
                    return ServiceResult<string>.Failure("Course not found.", ServiceErrorCode.NotFound);

                if (form.ContainsKey("Name") && !string.IsNullOrWhiteSpace(request.Name)) course.Name = request.Name;
                if (form.ContainsKey("Description")) course.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description;
                if (form.ContainsKey("Instructor")) course.Instructor = string.IsNullOrWhiteSpace(request.Instructor) ? null : request.Instructor;
                if (form.ContainsKey("ExternalUrl") && !string.IsNullOrWhiteSpace(request.ExternalUrl)) course.ExternalUrl = request.ExternalUrl;
                if (form.ContainsKey("Price")) course.Price = request.Price;
                if (form.ContainsKey("DurationHours")) course.DurationHours = request.DurationHours;
                if (form.ContainsKey("DifficultyLevel")) course.DifficultyLevel = string.IsNullOrWhiteSpace(request.DifficultyLevel) ? null : request.DifficultyLevel;
                if (form.ContainsKey("Rating")) course.Rating = request.Rating;
                if (form.ContainsKey("ThumbnailUrl")) course.ThumbnailUrl = string.IsNullOrWhiteSpace(request.ThumbnailUrl) ? null : request.ThumbnailUrl;

                if (form.ContainsKey("PlatformId") && request.PlatformId.HasValue)
                {
                    var platformExists = await _platformRepo.AnyAsync(p => p.Id == request.PlatformId.Value);
                    if (!platformExists)
                        return ServiceResult<string>.Failure("Selected platform does not exist.", ServiceErrorCode.ValidationError);
                    course.PlatformId = request.PlatformId.Value;
                }

                if (form.ContainsKey("CategoryId"))
                    course.CategoryId = string.IsNullOrWhiteSpace(form["CategoryId"]) ? null : (int.TryParse(form["CategoryId"], out int catId) ? catId : null);

                if (form.ContainsKey("SubCategoryId"))
                    course.SubCategoryId = string.IsNullOrWhiteSpace(form["SubCategoryId"]) ? null : (int.TryParse(form["SubCategoryId"], out int subCatId) ? subCatId : null);

                if (course.CategoryId.HasValue && course.SubCategoryId.HasValue)
                {
                    var isValidSubCategory = await _subCategoryRepo.AnyAsync(sc =>
                        sc.Id == course.SubCategoryId && sc.CategoryId == course.CategoryId);

                    if (!isValidSubCategory)
                        return ServiceResult<string>.Failure("SubCategory does not match Category.", ServiceErrorCode.ValidationError);
                }

                course.IsFree = (course.Price == null || course.Price == 0m);
                course.LastUpdatedAt = DateTime.UtcNow;

                _courseRepo.Update(course);
                await _courseRepo.SaveChangesAsync();

                try
                {
                    var enrolledUserIds = await _progressRepo.Query()
                        .Where(p => p.CourseId == course.Id)
                        .Select(p => p.UserId)
                        .Distinct()
                        .ToListAsync();

                    if (enrolledUserIds.Any())
                    {
                        var title = $"Course updated: {course.Name}";
                        var message = "A course you are enrolled in has been updated. Check what's new!";

                        await _notificationService.CreateForUsersAsync(
                            enrolledUserIds,
                            "CourseUpdate",
                            title,
                            message,
                            "Course",
                            course.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to create/publish course update notifications for CourseId {CourseId}", course.Id);
                }

                return ServiceResult<string>.Success("Course updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course {Id}", id);
                return ServiceResult<string>.Failure("An error occurred while updating the course.", ServiceErrorCode.UpstreamServiceError);
            }
        }


        public async Task<ServiceResult<string>> DeleteCourseAsync(int id)
        {
            try
            {
                var course = await _courseRepo.GetByIdAsync(id);
                if (course == null)
                    return ServiceResult<string>.Failure("Course not found.", ServiceErrorCode.NotFound);

                _courseRepo.Remove(course);
                await _courseRepo.SaveChangesAsync();

                return ServiceResult<string>.Success("Course deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {Id}", id);
                return ServiceResult<string>.Failure("An error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }
    }

    public class AiExtractedSkill
    {
        public string SkillName { get; set; }
        public string SkillLevel { get; set; }
    }
}
