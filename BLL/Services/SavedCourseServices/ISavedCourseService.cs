using BLL.Common;
using BLL.Dtos.SavedCourseDtos;

namespace BLL.Services.SavedCourseServices
{
    public interface ISavedCourseService
    {
        Task<ServiceResult<string>> SaveCourseAsync(string userId, SavedCourseRQ request);
        Task<ServiceResult<List<SavedCourseRS>>> GetSavedCoursesAsync(string userId);
        Task<ServiceResult<string>> RemoveSavedCourseAsync(string userId, int savedcourseId);
        Task<ServiceResult<bool>> IsSavedAsync(string userId, int courseId);
    }
}
