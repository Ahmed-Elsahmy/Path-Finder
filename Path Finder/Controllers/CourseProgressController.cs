using BLL.Common;
using BLL.Dtos.CourseProgressDtos;
using BLL.Services.CourseProgressService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class CourseProgressController : ControllerBase
    {
        private readonly ICourseProgressService _progressService;

        public CourseProgressController(ICourseProgressService progressService)
        {
            _progressService = progressService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue("uid")
                   ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
                return Ok(new { Data = result.Data, Message = result.Data as string });

            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }
        [HttpGet("my-progress")]
        public async Task<IActionResult> GetMyProgress()
        {
            var userId = GetUserId();
            var result = await _progressService.GetUserProgressAsync(userId);

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }


        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollInCourse([FromBody] EnrollCourseRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _progressService.EnrollInCourseAsync(userId, request);
            return HandleResult(result);
        }
        [HttpPut("UpdateProgress/{progressId}")]
        public async Task<IActionResult> UpdateProgress(int progressId, [FromBody] UpdateProgressRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _progressService.UpdateProgressAsync(userId, progressId, request);
            return HandleResult(result);
        }
        [HttpDelete("drop/{progressId}")]
        public async Task<IActionResult> DropCourse(int progressId)
        {
            var userId = GetUserId();
            var result = await _progressService.DropCourseAsync(userId, progressId);
            return HandleResult(result);
        }
    }
}