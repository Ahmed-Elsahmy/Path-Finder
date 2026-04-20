using BLL.Common;
using BLL.Dtos.CourseDtos;

namespace BLL.Services.CourseRecommendationService
{
    public interface ICourseRecommendationService
    {
        /// <summary>AI-powered course recommendations based on user's skills, career goals, and skill gaps</summary>
        Task<ServiceResult<CourseRecommendationListRS>> GetRecommendationsAsync(
            string userId,
            string? targetJobTitle = null,
            CancellationToken cancellationToken = default);
    }
}
