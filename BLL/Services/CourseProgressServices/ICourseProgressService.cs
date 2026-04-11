using BLL.Common;
using BLL.Dtos.CourseProgressDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.CourseProgressService
{
    public interface ICourseProgressService
    {
        Task<ServiceResult<List<CourseProgressRS>>> GetUserProgressAsync(string userId);
        Task<ServiceResult<CourseProgressRS>> GetProgressByIdAsync(string userId, int progressId);
        Task<ServiceResult<string>> EnrollInCourseAsync(string userId, EnrollCourseRQ request);
        Task<ServiceResult<string>> UpdateProgressAsync(string userId, int progressId, UpdateProgressRQ request);
        Task<ServiceResult<string>> DropCourseAsync(string userId, int progressId);
    }
}