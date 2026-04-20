using BLL.Common;
using BLL.Services.CourseRecommendationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseRecommendationController : ControllerBase
    {
        private readonly ICourseRecommendationService _recommendationService;

        public CourseRecommendationController(ICourseRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        private string? GetUserId() => User.FindFirst("uid")?.Value;

        /// <summary>Get personalized AI course recommendations based on your skills and career goals</summary>
        [HttpGet("my-recommendations")]
        public async Task<IActionResult> GetRecommendations(
            [FromQuery][StringLength(200)] string? targetJobTitle,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _recommendationService.GetRecommendationsAsync(
                userId, targetJobTitle, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ServiceErrorCode.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                    _ => BadRequest(new { Message = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }
    }
}
