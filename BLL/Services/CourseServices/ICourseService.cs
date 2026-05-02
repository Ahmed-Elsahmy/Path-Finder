using BLL.Common;
using BLL.Dtos.CourseDtos;
using Microsoft.AspNetCore.Http;

namespace BLL.Services.CourseService
{
    public interface ICourseService
    {
        Task<ServiceResult<List<CourseRS>>> GetCoursesAsync(CourseFilterRQ filter);
        Task<ServiceResult<CourseRS>> GetCourseByIdAsync(int id);
        Task<ServiceResult<List<CourseRS>>> SearchCoursesAsync(string name, string? userId = null);
        Task<ServiceResult<string>> CreateCourseAsync(CourseRQ request);
        Task<ServiceResult<string>> UpdateCourseAsync(int id, UpdateCourseRQ request, IFormCollection form);
        Task<ServiceResult<string>> DeleteCourseAsync(int id);
    }
}