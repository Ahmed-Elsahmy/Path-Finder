using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Common;
using BLL.Dtos.RecentSearchDtos;
using DAL.Helper.Enums;

namespace BLL.Services.RecentSearchServices
{
    public interface IRecentSearchService
    {
        Task<ServiceResult<string>> AddSearchAsync(string userId, string term, RecentSearchType type);
        Task<ServiceResult<List<RecentSearchRS>>> GetRecentJobsAsync(string userId);
        Task<ServiceResult<List<RecentSearchRS>>> GetRecentCoursesAsync(string userId);
        Task<ServiceResult<List<RecentSearchRS>>> GetRecentCareerPathsAsync(string userId);
        Task<ServiceResult<string>> ClearRecentJobsAsync(string userId);
        Task<ServiceResult<string>> ClearRecentCoursesAsync(string userId);
        Task<ServiceResult<string>> ClearRecentCareerPathsAsync(string userId);
        Task<ServiceResult<string>>ClearRecentSearchByIdAsync(string userId, int id);
    }
}
