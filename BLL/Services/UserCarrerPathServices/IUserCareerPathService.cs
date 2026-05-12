using BLL.Common;
using BLL.Dtos.UserCarrerPathDtos;

namespace BLL.Services.UserCarrerPathServices
{
    public interface IUserCareerPathService
    {
        Task<ServiceResult<UserCareerPathRS>> EnrollInCareerPathAsync(string userId, UserCareerPathRQ request);
        Task<ServiceResult<string>> UnenrollFromCareerPathAsync(string userId, int userCareerPathId);
        Task<ServiceResult<List<UserCareerPathRS>>> GetUserCareerPathsAsync(string userId);
        Task<ServiceResult<UserCareerPathRS>> GetUserCareerPathByIdAsync(string userId, int userCareerPathId);
        Task<ServiceResult<CareerPathRecommendationListRS>> GetRecommendationsAsync(
              string userId,
              string? targetJobTitle = null,
              CancellationToken cancellationToken = default);
        Task<ServiceResult<List<UserCareerPathRS>>> GetCareerPathsAsync(string userId, UserCareerPathFilter filter);

    }
}
