using AutoMapper;
using BLL.Common;
using BLL.Dtos.JobDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.SavedJobServices
{
    public class SavedJobService : ISavedJobService
    {
        private readonly IRepository<SavedJob> _savedJobRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SavedJobService> _logger;

        public SavedJobService(
            IRepository<SavedJob> savedJobRepository,
            IRepository<Job> jobRepository,
            IMapper mapper,
            ILogger<SavedJobService> logger)
        {
            _savedJobRepository = savedJobRepository;
            _jobRepository = jobRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> SaveJobAsync(string userId, SaveJobRQ request)
        {
            try
            {
                var job = await _jobRepository.GetByIdAsync(request.JobId);
                if (job == null || !job.IsActive)
                    return ServiceResult<string>.Failure("Job not found or is no longer active.");

                var alreadySaved = await _savedJobRepository.AnyAsync(
                    sj => sj.UserId == userId && sj.JobId == request.JobId);
                if (alreadySaved)
                    return ServiceResult<string>.Failure("Job is already saved.");

                var savedJob = new SavedJob
                {
                    UserId = userId,
                    JobId = request.JobId,
                    Notes = request.Notes,
                    SavedAt = DateTime.UtcNow
                };

                await _savedJobRepository.AddAsync(savedJob);
                await _savedJobRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Job saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving job {JobId} for user {UserId}", request.JobId, userId);
                return ServiceResult<string>.Failure($"Error saving job: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<SavedJobRS>>> GetSavedJobsAsync(string userId)
        {
            try
            {
                var items = await _savedJobRepository.Query()
                    .Where(sj => sj.UserId == userId)
                    .Include(sj => sj.Job)
                    .OrderByDescending(sj => sj.SavedAt)
                    .ToListAsync();

                return ServiceResult<List<SavedJobRS>>.Success(_mapper.Map<List<SavedJobRS>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving saved jobs for user {UserId}", userId);
                return ServiceResult<List<SavedJobRS>>.Failure("Error retrieving saved jobs.");
            }
        }

        public async Task<ServiceResult<string>> RemoveSavedJobAsync(string userId, int savedJobId)
        {
            try
            {
                var savedJob = await _savedJobRepository.FirstOrDefaultAsync(
                    sj => sj.SavedJobId == savedJobId && sj.UserId == userId);

                if (savedJob == null)
                    return ServiceResult<string>.Failure("Saved job not found.");

                _savedJobRepository.Remove(savedJob);
                await _savedJobRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Saved job removed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing saved job {SavedJobId}", savedJobId);
                return ServiceResult<string>.Failure($"Error removing saved job: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> IsSavedAsync(string userId, int jobId)
        {
            try
            {
                var isSaved = await _savedJobRepository.AnyAsync(
                    sj => sj.UserId == userId && sj.JobId == jobId);
                return ServiceResult<bool>.Success(isSaved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking saved status for job {JobId}", jobId);
                return ServiceResult<bool>.Failure("Error checking saved status.");
            }
        }
    }
}
