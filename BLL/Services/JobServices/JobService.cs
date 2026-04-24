using AutoMapper;
using BLL.Common;
using BLL.Dtos.JobDtos;
using BLL.Services.NotificationServices;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BLL.Services.JobServices
{
    public class JobService : IJobService
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<JobSkillRequirement> _skillReqRepository;
        private readonly IRepository<Skill> _skillRepository;
        private readonly IRepository<UserSkill> _userSkillRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<JobService> _logger;

        public JobService(
            IRepository<Job> jobRepository,
            IRepository<JobSkillRequirement> skillReqRepository,
            IRepository<Skill> skillRepository,
            IRepository<UserSkill> userSkillRepository,
            INotificationService notificationService,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IMapper mapper,
            ILogger<JobService> logger)
        {
            _jobRepository = jobRepository;
            _skillReqRepository = skillReqRepository;
            _skillRepository = skillRepository;
            _userSkillRepository = userSkillRepository;
            _notificationService = notificationService;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<JobRS>>> GetJobsAsync(JobFilterRQ filter)
        {
            try
            {
                var query = _jobRepository.Query()
                    .Include(j => j.Source)
                    .Include(j => j.SkillRequirements)
                        .ThenInclude(sr => sr.Skill)
                    .AsQueryable();
                  if(filter.IsActive)
                    query = query.Where(j => j.IsActive);
                else if (!filter.IsActive)
                    query = query.Where(j => j.IsActive == false);
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var term = filter.SearchTerm.ToLower();
                    query = query.Where(j =>
                        j.JobTitle.ToLower().Contains(term) ||
                        (j.CompanyName != null && j.CompanyName.ToLower().Contains(term)) ||
                        (j.Description != null && j.Description.ToLower().Contains(term)));
                }

                if (!string.IsNullOrWhiteSpace(filter.Location))
                    query = query.Where(j => j.Location != null && j.Location.ToLower().Contains(filter.Location.ToLower()));

                if (!string.IsNullOrWhiteSpace(filter.JobType))
                    query = query.Where(j => j.JobType == filter.JobType);

                if (!string.IsNullOrWhiteSpace(filter.ExperienceLevel))
                    query = query.Where(j => j.ExperienceLevel == filter.ExperienceLevel);

                if (filter.SourceId.HasValue)
                    query = query.Where(j => j.SourceId == filter.SourceId.Value);

                if (filter.MinSalary.HasValue)
                    query = query.Where(j => j.SalaryMax >= filter.MinSalary.Value);

                var items = await query
                    .OrderByDescending(j => j.PostedDate)
                    .ToListAsync();

                return ServiceResult<List<JobRS>>.Success(_mapper.Map<List<JobRS>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs");
                return ServiceResult<List<JobRS>>.Failure("Error retrieving jobs.");
            }
        }

        public async Task<ServiceResult<JobRS>> GetJobByIdAsync(int id)
        {
            try
            {
                var job = await _jobRepository.Query()
                    .Include(j => j.Source)
                    .Include(j => j.SkillRequirements)
                        .ThenInclude(sr => sr.Skill)
                    .FirstOrDefaultAsync(j => j.JobId == id);

                if (job == null)
                    return ServiceResult<JobRS>.Failure("Job not found.");

                return ServiceResult<JobRS>.Success(_mapper.Map<JobRS>(job));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job {JobId}", id);
                return ServiceResult<JobRS>.Failure("Error retrieving job.");
            }
        }

        public async Task<ServiceResult<string>> CreateJobAsync(JobRQ request)
        {
            try
            {
                var job = _mapper.Map<Job>(request);
                job.PostedDate = DateTime.UtcNow;

                await _jobRepository.AddAsync(job);
                await _jobRepository.SaveChangesAsync();

                // AI: Extract skills INLINE (not background)
                if (!string.IsNullOrWhiteSpace(request.Description) || !string.IsNullOrWhiteSpace(request.JobTitle))
                {
                    try
                    {
                        await ExtractAndLinkSkillsAsync(job.JobId, request.JobTitle, request.Description);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "AI skill extraction failed for job {JobId}, job still created", job.JobId);
                    }
                }

                try
                {
                    var requiredSkillIds = await _skillReqRepository.Query()
                        .Where(sr => sr.JobId == job.JobId)
                        .Select(sr => sr.SkillId)
                        .Distinct()
                        .ToListAsync();

                    if (requiredSkillIds.Any())
                    {
                        var matchedUsers = await _userSkillRepository.Query()
                            .Where(us => requiredSkillIds.Contains(us.SkillId))
                            .GroupBy(us => us.UserId)
                            .Select(g => new
                            {
                                UserId = g.Key,
                                MatchCount = g.Count()
                            })
                            .OrderByDescending(x => x.MatchCount)
                            .Take(200)
                            .ToListAsync();

                        if (matchedUsers.Any())
                        {
                            var title = $"🔥 New Job Match: {job.JobTitle}";

                            var parts = new List<string>();

                            if (!string.IsNullOrWhiteSpace(job.CompanyName))
                                parts.Add($"🏢 {job.CompanyName}");

                            if (!string.IsNullOrWhiteSpace(job.Location))
                                parts.Add($"📍 {job.Location}");

                            if (!string.IsNullOrWhiteSpace(job.JobType))
                                parts.Add($"💼 {job.JobType}");

                            if (job.SalaryMin.HasValue || job.SalaryMax.HasValue)
                            {
                                var salary = $"{job.SalaryMin?.ToString("N0") ?? "?"} - {job.SalaryMax?.ToString("N0") ?? "?"}";
                                parts.Add($"💰 {salary}");
                            }

                            var details = parts.Any() ? string.Join(" | ", parts) : null;

                            // 🔥 send per-user notification with match strength
                            foreach (var user in matchedUsers)
                            {
                                try
                                {
                                    var matchInfo = $"🔥 {user.MatchCount} skill(s) matched";

                                    var message = details != null
                                        ? $"{job.JobTitle}\n{details}\n\n{matchInfo}\n✨ Matches your profile. Check it out!"
                                        : $"{job.JobTitle}\n\n{matchInfo}\n✨ Matches your profile. Check it out!";

                                    await _notificationService.CreateForUserAsync(
                                        user.UserId,
                                        "JobMatch",
                                        title,
                                        message,
                                        "Job",
                                        job.JobId);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex,
                                        "Failed to notify user {UserId} about job {JobId}",
                                        user.UserId,
                                        job.JobId);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to create/publish job match notifications for JobId {JobId}", job.JobId);
                }

                return ServiceResult<string>.Success("Job created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job");
                return ServiceResult<string>.Failure($"Error creating job: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> UpdateJobAsync(int id, UpdateJobRQ request)
        {
            try
            {
                var job = await _jobRepository.GetByIdAsync(id);
                if (job == null)
                    return ServiceResult<string>.Failure("Job not found.");

                if (!string.IsNullOrWhiteSpace(request.JobTitle))
                    job.JobTitle = request.JobTitle;
                if (!string.IsNullOrWhiteSpace(request.CompanyName))
                    job.CompanyName = request.CompanyName;
                if (request.Description != null)
                    job.Description = request.Description;
                if (!string.IsNullOrWhiteSpace(request.Location))
                    job.Location = request.Location;
                if (!string.IsNullOrWhiteSpace(request.JobType))
                    job.JobType = request.JobType;
                if (!string.IsNullOrWhiteSpace(request.ExperienceLevel))
                    job.ExperienceLevel = request.ExperienceLevel;
                if (request.SalaryMin.HasValue)
                    job.SalaryMin = request.SalaryMin;
                if (request.SalaryMax.HasValue)
                    job.SalaryMax = request.SalaryMax;
                if (!string.IsNullOrWhiteSpace(request.ExternalUrl))
                    job.ExternalUrl = request.ExternalUrl;
                if (request.ExpiryDate.HasValue)
                    job.ExpiryDate = request.ExpiryDate;
                if (!string.IsNullOrWhiteSpace(request.ContactInfo))
                    job.ContactInfo = request.ContactInfo;
                if (request.IsActive.HasValue)
                    job.IsActive = request.IsActive.Value;

                await _jobRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Job updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job {JobId}", id);
                return ServiceResult<string>.Failure($"Error updating job: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> DeleteJobAsync(int id)
        {
            try
            {
                var job = await _jobRepository.GetByIdAsync(id);
                if (job == null)
                    return ServiceResult<string>.Failure("Job not found.");

                job.IsActive = false;
                await _jobRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Job deactivated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId}", id);
                return ServiceResult<string>.Failure($"Error deleting job: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<JobRS>>> GetRecommendedJobsAsync(string userId)
        {
            try
            {
                var userSkillIds = await _userSkillRepository.Query()
                    .Where(us => us.UserId == userId)
                    .Select(us => us.SkillId)
                    .Distinct()
                    .ToListAsync();

                if (!userSkillIds.Any())
                    return ServiceResult<List<JobRS>>.Failure(
                        "You don't have any skills yet. Add skills or upload a CV to get job recommendations.");

                var items = await _jobRepository.Query()
                    .Include(j => j.Source)
                    .Include(j => j.SkillRequirements)
                        .ThenInclude(sr => sr.Skill)
                    .Where(j => j.IsActive)
                    .Where(j => j.SkillRequirements.Any(sr => userSkillIds.Contains(sr.SkillId)))
                    .OrderByDescending(j => j.SkillRequirements.Count(sr => userSkillIds.Contains(sr.SkillId)))
                    .ThenByDescending(j => j.PostedDate)
                    .ToListAsync();

                return ServiceResult<List<JobRS>>.Success(_mapper.Map<List<JobRS>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended jobs for user {UserId}", userId);
                return ServiceResult<List<JobRS>>.Failure("Error getting job recommendations.");
            }
        }
        private async Task ExtractAndLinkSkillsAsync(int jobId, string title, string? description)
        {
            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey)) return;

            var prompt = $@"
Extract the technical and soft skills required for this job. Return ONLY a JSON array of short, canonical skill names.
Use standard names (e.g., ""C#"" not ""C Sharp"", ""React"" not ""ReactJS Development"").

Job Title: {title}
Description: {description ?? "N/A"}";

            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new
                {
                    temperature = 0.2,
                    responseMimeType = "application/json"
                }
            };

            var client = _httpClientFactory.CreateClient("GeminiClient");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", apiKey);

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Gemini skill extraction returned {Status}", response.StatusCode);
                return;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var aiText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString();

            if (string.IsNullOrWhiteSpace(aiText)) return;

            aiText = aiText.Replace("```json", "").Replace("```", "").Trim();
            var skills = JsonSerializer.Deserialize<List<string>>(aiText);
            if (skills == null || !skills.Any()) return;

            var allGlobalSkills = await _skillRepository.GetAllAsync();

            foreach (var skillName in skills.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                var skillLower = skillName.ToLower().Trim();

                var globalSkill = allGlobalSkills.FirstOrDefault(s =>
                {
                    var gl = s.SkillName.ToLower().Trim();
                    return gl == skillLower || gl.Contains(skillLower) || skillLower.Contains(gl);
                });

                if (globalSkill == null)
                {
                    globalSkill = new Skill { SkillName = skillName, Category = "Job Requirement", IsTechnical = true };
                    await _skillRepository.AddAsync(globalSkill);
                    await _skillRepository.SaveChangesAsync();
                    allGlobalSkills.Add(globalSkill);
                }

                var exists = await _skillReqRepository.AnyAsync(
                    sr => sr.JobId == jobId && sr.SkillId == globalSkill.SkillId);
                if (!exists)
                {
                    await _skillReqRepository.AddAsync(new JobSkillRequirement
                    {
                        JobId = jobId,
                        SkillId = globalSkill.SkillId,
                        IsMandatory = true
                    });
                }
            }

            await _skillReqRepository.SaveChangesAsync();
            _logger.LogInformation("Extracted {Count} skills for job {JobId}", skills.Count, jobId);
        }
    }
}
