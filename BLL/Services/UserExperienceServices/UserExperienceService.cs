using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Common;
using BLL.Dtos.EducationDtos;
using BLL.Dtos.UserExperienceDtos;
using BLL.Dtos.UserProfileDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace BLL.Services.UserExperienceServices
{
    public class UserExperienceService : IUserExperienceService
    {

        private readonly IRepository<UserExperience> _experienceRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ILogger<UserExperienceService> _logger;
        public UserExperienceService(
            IRepository<UserExperience> experienceRepository,
            IWebHostEnvironment env,
            IMapper mapper,
            ILogger<UserExperienceService> logger)
        {
            _experienceRepository = experienceRepository;
            _env = env;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ServiceResult<string>> AddExperienceAsync(string userId, UserExperienceRQ request)
        {
            try
            {
                var experience = _mapper.Map<UserExperience>(request);
                experience.UserId = userId;
                if (experience.IsCurrent)
                {
                    experience.EndDate = null;
                }
                if (experience.StartDate.HasValue && experience.EndDate.HasValue)
                {
                    if (experience.StartDate > experience.EndDate)
                    {
                        return ServiceResult<string>
                            .Failure("Start date cannot be greater than end date.");
                    }
                }
                if (experience.IsCurrent)
                {
                    var currentExperiences = await _experienceRepository
                        .FindAsync(e => e.UserId == userId && e.IsCurrent);

                    foreach (var exp in currentExperiences)
                    {
                        exp.IsCurrent = false;
                        exp.EndDate ??= DateTime.UtcNow;
                    }
                }
                await _experienceRepository.AddAsync(experience);
                await _experienceRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Experience added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding experience for user {UserId}", userId);

                return ServiceResult<string>
                    .Failure("An error occurred while adding the experience.");
            }
        }

        public async Task<ServiceResult<List<UserExperienceRS>>> GetUserExperiencesAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<List<UserExperienceRS>>
                    .Failure("Invalid user ID.");
            }
            try
            {
                var experiences = await _experienceRepository
                    .FindAsync(e => e.UserId == userId);

                var ordered = experiences
                    .OrderByDescending(e => e.IsCurrent)
                    .ThenByDescending(e => e.StartDate)
                    .ToList();

                var result = _mapper.Map<List<UserExperienceRS>>(ordered);
                return ServiceResult<List<UserExperienceRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving experiences for user {UserId}", userId);

                return ServiceResult<List<UserExperienceRS>>
                    .Failure("An error occurred while retrieving your experience records.");
            }
        }

        public async Task<ServiceResult<string>> UpdateExperienceAsync(
     string userId,
     int experienceId,
     UpdateUserExperienceRQ request)
        {
            try
            {
                var experience = await _experienceRepository
                    .FirstOrDefaultAsync(e => e.ExperienceId == experienceId && e.UserId == userId);
                if (experience == null)
                {
                    return ServiceResult<string>.Failure("Experience not found or does not belong to you.");
                }
                _mapper.Map(request, experience);
                if (experience.StartDate.HasValue && experience.EndDate.HasValue)
                {
                    if (experience.StartDate > experience.EndDate)
                    {
                        return ServiceResult<string>
                            .Failure("Start date cannot be greater than end date.");
                    }
                }
                if(experience.EndDate<DateTime.UtcNow)
                {
                   experience.IsCurrent = false;
                }
                if (experience.IsCurrent)
                {
                    var currentExperiences = await _experienceRepository
                        .FindAsync(e => e.UserId == userId && e.IsCurrent && e.ExperienceId != experienceId);

                    foreach (var exp in currentExperiences)
                    {
                        exp.IsCurrent = false;
                        exp.EndDate ??= DateTime.UtcNow;
                    }
                }
                await _experienceRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Experience updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating experience {ExperienceId} for user {UserId}", experienceId, userId);

                return ServiceResult<string>
                    .Failure("An error occurred while updating the experience.");
            }
        }
        public async Task<ServiceResult<string>> DeleteExperienceAsync(string userId, int experienceId)
        {
         
            try
            {
                var experience = await _experienceRepository
                    .FirstOrDefaultAsync(e => e.ExperienceId == experienceId && e.UserId == userId);
                if (experience == null)
                {
                    return ServiceResult<string>.Failure("Experience not found or does not belong to you.");
                }
                 _experienceRepository.Remove(experience);
                await _experienceRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Experience deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting experience {ExperienceId} for user {UserId}", experienceId, userId);

                return ServiceResult<string>
                    .Failure("An error occurred while deleting the experience.");
            }
        }
    }
}
