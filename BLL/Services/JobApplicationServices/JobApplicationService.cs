using AutoMapper;
using BLL.Common;
using BLL.Dtos.JobDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BLL.Services.JobApplicationServices
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IRepository<JobApplication> _appRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<UserSkill> _userSkillRepository;
        private readonly IRepository<JobSkillRequirement> _skillReqRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<JobApplicationService> _logger;

        public JobApplicationService(
            IRepository<JobApplication> appRepository,
            IRepository<Job> jobRepository,
            IRepository<UserSkill> userSkillRepository,
            IRepository<JobSkillRequirement> skillReqRepository,
            IMapper mapper,
            ILogger<JobApplicationService> logger)
        {
            _appRepository = appRepository;
            _jobRepository = jobRepository;
            _userSkillRepository = userSkillRepository;
            _skillReqRepository = skillReqRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<JobApplicationRS>> ApplyToJobAsync(string userId, ApplyJobRQ request)
        {
            try
            {
                var job = await _jobRepository.Query()
                    .Include(j => j.SkillRequirements)
                        .ThenInclude(sr => sr.Skill)
                    .FirstOrDefaultAsync(j => j.JobId == request.JobId);

                if (job == null || !job.IsActive)
                    return ServiceResult<JobApplicationRS>.Failure("Job not found or is no longer active.");

                var alreadyApplied = await _appRepository.AnyAsync(
                    a => a.UserId == userId && a.JobId == request.JobId);
                if (alreadyApplied)
                    return ServiceResult<JobApplicationRS>.Failure("You have already applied to this job.");

                // Calculate match percentage
                var (matchPercentage, skillGap) = await CalculateMatchAsync(userId, job);

                var application = new JobApplication
                {
                    UserId = userId,
                    JobId = request.JobId,
                    Notes = request.Notes,
                    AppliedAt = DateTime.UtcNow,
                    Status = "Applied",
                    MatchPercentage = matchPercentage,
                    SkillGapAnalysis = skillGap
                };

                await _appRepository.AddAsync(application);
                await _appRepository.SaveChangesAsync();

                // Re-fetch with navigation for mapping
                var saved = await _appRepository.Query()
                    .Include(a => a.Job)
                    .FirstOrDefaultAsync(a => a.ApplicationId == application.ApplicationId);

                return ServiceResult<JobApplicationRS>.Success(_mapper.Map<JobApplicationRS>(saved));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying to job {JobId} for user {UserId}", request.JobId, userId);
                return ServiceResult<JobApplicationRS>.Failure($"Error applying to job: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<JobApplicationRS>>> GetUserApplicationsAsync(string userId)
        {
            try
            {
                var items = await _appRepository.Query()
                    .Where(a => a.UserId == userId)
                    .Include(a => a.Job)
                    .OrderByDescending(a => a.AppliedAt)
                    .ToListAsync();

                return ServiceResult<List<JobApplicationRS>>.Success(
                    _mapper.Map<List<JobApplicationRS>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving applications for user {UserId}", userId);
                return ServiceResult<List<JobApplicationRS>>.Failure("Error retrieving applications.");
            }
        }

        public async Task<ServiceResult<string>> UpdateApplicationAsync(
            string userId, int applicationId, UpdateApplicationRQ request)
        {
            try
            {
                var app = await _appRepository.FirstOrDefaultAsync(
                    a => a.ApplicationId == applicationId && a.UserId == userId);

                if (app == null)
                    return ServiceResult<string>.Failure("Application not found.");

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    var validStatuses = new[] { "Applied", "Interviewing", "Rejected", "Accepted" };
                    if (!validStatuses.Contains(request.Status))
                        return ServiceResult<string>.Failure(
                            $"Invalid status. Valid values: {string.Join(", ", validStatuses)}");
                    app.Status = request.Status;
                }

                if (!string.IsNullOrWhiteSpace(request.Notes))
                    app.Notes = request.Notes;

                await _appRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Application updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application {AppId}", applicationId);
                return ServiceResult<string>.Failure($"Error updating application: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> WithdrawApplicationAsync(string userId, int applicationId)
        {
            try
            {
                var app = await _appRepository.FirstOrDefaultAsync(
                    a => a.ApplicationId == applicationId && a.UserId == userId);

                if (app == null)
                    return ServiceResult<string>.Failure("Application not found.");

                _appRepository.Remove(app);
                await _appRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Application withdrawn.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error withdrawing application {AppId}", applicationId);
                return ServiceResult<string>.Failure($"Error withdrawing application: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════
        // Skill Match Calculator
        // ═══════════════════════════════════════════

        private async Task<(float Percentage, string GapAnalysis)> CalculateMatchAsync(string userId, Job job)
        {
            try
            {
                var userSkillIds = await _userSkillRepository.Query()
                    .Where(us => us.UserId == userId)
                    .Select(us => us.SkillId)
                    .Distinct()
                    .ToListAsync();

                var requirements = job.SkillRequirements.ToList();

                if (!requirements.Any())
                    return (0, "No skill requirements specified for this job.");

                var matchedSkills = requirements
                    .Where(r => userSkillIds.Contains(r.SkillId))
                    .Select(r => r.Skill.SkillName)
                    .ToList();

                var missingSkills = requirements
                    .Where(r => !userSkillIds.Contains(r.SkillId))
                    .Select(r => r.Skill.SkillName)
                    .ToList();

                // Weighted: mandatory skills count double
                var totalWeight = requirements.Sum(r => r.IsMandatory ? 2.0f : 1.0f);
                var matchedWeight = requirements
                    .Where(r => userSkillIds.Contains(r.SkillId))
                    .Sum(r => r.IsMandatory ? 2.0f : 1.0f);

                var percentage = totalWeight > 0 ? (matchedWeight / totalWeight) * 100f : 0f;

                var gap = new StringBuilder();
                gap.AppendLine($"Match: {percentage:F0}% ({matchedSkills.Count}/{requirements.Count} skills)");

                if (matchedSkills.Any())
                    gap.AppendLine($"Matched: {string.Join(", ", matchedSkills)}");

                if (missingSkills.Any())
                    gap.AppendLine($"Missing: {string.Join(", ", missingSkills)}");

                return (percentage, gap.ToString().Trim());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error calculating match for user {UserId}", userId);
                return (0, "Could not calculate skill match.");
            }
        }
    }
}
