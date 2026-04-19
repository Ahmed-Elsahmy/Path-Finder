using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Task<ServiceResult<bool>> IsUserEnrolledAsync(string userId, int careerPathId);
        Task<ServiceResult<List<RecommendedCareerPathDto>>> GetRecommendedCareerPathsAsync(string userId);
        Task<ServiceResult<List<UserCareerPathRS>>> GetCareerPathsAsync(string userId, UserCareerPathFilter filter);

    }
}
