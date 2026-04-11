using AutoMapper;
using BLL.Common;
using BLL.Dtos.CoursePlatformDtos;
using BLL.Services.CoursePlatformServices;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Services.CoursePlatformService
{
    public class CoursePlatformService : ICoursePlatformService
    {
        private readonly IRepository<CoursePlatform> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CoursePlatformService> _logger;

        public CoursePlatformService(
            IRepository<CoursePlatform> repository,
            IMapper mapper,
            ILogger<CoursePlatformService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<CoursePlatformRS>>> GetAllPlatformsAsync(bool onlyActive = false)
        {
            try
            {
                var platforms = onlyActive ? await _repository.FindAsync(p => p.IsActive) : await _repository.GetAllAsync();

                var result = _mapper.Map<List<CoursePlatformRS>>(platforms);
                return ServiceResult<List<CoursePlatformRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching course platforms.");
                return ServiceResult<List<CoursePlatformRS>>.Failure("An error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<CoursePlatformRS>> GetPlatformByIdAsync(int id)
        {
            try
            {
                var platform = await _repository.GetByIdAsync(id);
                if (platform == null)
                    return ServiceResult<CoursePlatformRS>.Failure("Platform not found.", ServiceErrorCode.NotFound);

                var result = _mapper.Map<CoursePlatformRS>(platform);
                return ServiceResult<CoursePlatformRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching platform {Id}", id);
                return ServiceResult<CoursePlatformRS>.Failure("An error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<string>> CreatePlatformAsync(CoursePlatformRQ request)
        {
            try
            {
                var platformExists = await _repository.AnyAsync(p => p.Name.ToLower() == request.Name.ToLower());
                if (platformExists)
                    return ServiceResult<string>.Failure("A platform with this name already exists.", ServiceErrorCode.ValidationError);

                var platform = _mapper.Map<CoursePlatform>(request);

                await _repository.AddAsync(platform);
                await _repository.SaveChangesAsync();

                return ServiceResult<string>.Success("Course platform created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course platform.");
                return ServiceResult<string>.Failure("An error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<string>> UpdatePlatformAsync(int id, UpdateCoursePlatformRQ request, IFormCollection form)
        {
            try
            {
                var platform = await _repository.GetByIdAsync(id);

                if (platform == null)
                    return ServiceResult<string>.Failure("Course platform not found.", ServiceErrorCode.NotFound);
                if (form.ContainsKey("Name"))
                {
                    if (string.IsNullOrWhiteSpace(request.Name))
                        return ServiceResult<string>.Failure("Name cannot be empty.", ServiceErrorCode.ValidationError);

                    if (request.Name.ToLower() != platform.Name.ToLower())
                    {
                        var nameExists = await _repository.AnyAsync(p => p.Name.ToLower() == request.Name.ToLower());
                        if (nameExists)
                            return ServiceResult<string>.Failure("Another platform with this name already exists.", ServiceErrorCode.ValidationError);
                    }
                    platform.Name = request.Name;
                }
                if (form.ContainsKey("BaseUrl"))
                {
                    if (string.IsNullOrWhiteSpace(request.BaseUrl))
                        return ServiceResult<string>.Failure("BaseUrl cannot be empty.", ServiceErrorCode.ValidationError);

                    platform.BaseUrl = request.BaseUrl;
                }
                if (form.ContainsKey("Description"))
                {
                    platform.Description = string.IsNullOrWhiteSpace(request.Description)
                        ? null
                        : request.Description;
                }

                if (form.ContainsKey("LogoUrl"))
                {
                    platform.LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl)
                        ? null
                        : request.LogoUrl;
                }

                if (form.ContainsKey("ApiEndPoint"))
                {
                    platform.ApiEndPoint = string.IsNullOrWhiteSpace(request.ApiEndPoint)
                        ? null
                        : request.ApiEndPoint;
                }

                if (form.ContainsKey("IsActive") && request.IsActive.HasValue)
                {
                    platform.IsActive = request.IsActive.Value;
                }

                await _repository.SaveChangesAsync();

                return ServiceResult<string>.Success("Course platform updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course platform {Id}", id);
                return ServiceResult<string>.Failure($"Error updating platform: {ex.Message}", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<string>> DeletePlatformAsync(int id)
        {
            try
            {
                var platform = await _repository.GetByIdAsync(id);
                if (platform == null)
                    return ServiceResult<string>.Failure("Platform not found.", ServiceErrorCode.NotFound);

                _repository.Remove(platform);
                await _repository.SaveChangesAsync();

                return ServiceResult<string>.Success("Course platform deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting platform {Id}", id);
                return ServiceResult<string>.Failure("An error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }
    }
}