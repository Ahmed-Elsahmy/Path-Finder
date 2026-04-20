using BLL.Common;
using BLL.Dtos.UserExperienceDtos;
using BLL.Dtos.UserProfileDtos;
using Microsoft.AspNetCore.Http;

namespace BLL.Services.UserExperienceServices
{
    public interface IUserExperienceService
    {
        Task<ServiceResult<List<UserExperienceRS>>> GetUserExperiencesAsync(string userId);
        Task<ServiceResult<string>> AddExperienceAsync(string userId, UserExperienceRQ request);
        Task<ServiceResult<string>> UpdateExperienceAsync(string userId, int experienceId, UpdateUserExperienceRQ request);
        Task<ServiceResult<string>> DeleteExperienceAsync(string userId, int experienceId);
    }
}
