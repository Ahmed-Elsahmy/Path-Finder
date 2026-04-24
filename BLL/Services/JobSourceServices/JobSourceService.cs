using AutoMapper;
using BLL.Common;
using BLL.Dtos.JobDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.JobSourceServices
{
    public class JobSourceService : IJobSourceService
    {
        private readonly IRepository<JobSource> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<JobSourceService> _logger;

        public JobSourceService(
            IRepository<JobSource> repository,
            IMapper mapper,
            ILogger<JobSourceService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<JobSourceRS>>> GetAllSourcesAsync(
            bool onlyActive = true)
        {
            try
            {
                var sources = await _repository.Query()
                    .AsNoTracking()
                    .Where(js => !onlyActive || js.IsActive)
                    .OrderBy(js => js.SourceName)
                    .Select(js => new JobSourceRS
                    {
                        SourceId = js.SourceId,
                        SourceName = js.SourceName,
                        IsActive = js.IsActive
                    })
                    .ToListAsync();

                return ServiceResult<List<JobSourceRS>>.Success(sources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job sources (OnlyActive: {OnlyActive})", onlyActive);

                return ServiceResult<List<JobSourceRS>>.Failure(
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        public async Task<ServiceResult<JobSourceRS>> GetSourceByIdAsync(int id)
        {
            try
            {
                var source = await _repository.Query()
                    .Include(js => js.Jobs)
                    .FirstOrDefaultAsync(js => js.SourceId == id);

                if (source == null)
                    return ServiceResult<JobSourceRS>.Failure("Job source not found.");

                return ServiceResult<JobSourceRS>.Success(_mapper.Map<JobSourceRS>(source));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job source {SourceId}", id);
                return ServiceResult<JobSourceRS>.Failure("Error retrieving job source.");
            }
        }

        public async Task<ServiceResult<string>> CreateSourceAsync(JobSourceRQ request)
        {
            try
            {
                var exists = await _repository.AnyAsync(js => js.SourceName == request.SourceName);
                if (exists)
                    return ServiceResult<string>.Failure("A job source with this name already exists.");

                var source = _mapper.Map<JobSource>(request);
                await _repository.AddAsync(source);
                await _repository.SaveChangesAsync();

                return ServiceResult<string>.Success("Job source created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job source");
                return ServiceResult<string>.Failure($"Error creating job source: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> UpdateSourceAsync(int id, UpdateJobSourceRQ request)
        {
            try
            {
                var source = await _repository.GetByIdAsync(id);
                if (source == null)
                    return ServiceResult<string>.Failure("Job source not found.");

                if (!string.IsNullOrWhiteSpace(request.SourceName))
                {
                    var nameExists = await _repository.AnyAsync(
                        js => js.SourceName == request.SourceName && js.SourceId != id);
                    if (nameExists)
                        return ServiceResult<string>.Failure("Another source with this name already exists.");
                    source.SourceName = request.SourceName;
                }

                if (!string.IsNullOrWhiteSpace(request.SourceType))
                    source.SourceType = request.SourceType;
                if (!string.IsNullOrWhiteSpace(request.APIEndpoint))
                    source.APIEndpoint = request.APIEndpoint;
                if (request.IsActive.HasValue)
                    source.IsActive = request.IsActive.Value;

                await _repository.SaveChangesAsync();
                return ServiceResult<string>.Success("Job source updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job source {SourceId}", id);
                return ServiceResult<string>.Failure($"Error updating job source: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> DeleteSourceAsync(int id)
        {
            try
            {
                var source = await _repository.GetByIdAsync(id);
                if (source == null)
                    return ServiceResult<string>.Failure("Job source not found.");

                _repository.Remove(source);
                await _repository.SaveChangesAsync();
                return ServiceResult<string>.Success("Job source deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job source {SourceId}", id);
                return ServiceResult<string>.Failure($"Error deleting job source: {ex.Message}");
            }
        }
    }
}
