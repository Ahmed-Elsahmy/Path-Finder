using BLL.Common;
using BLL.Dtos.UserCarrerPathDtos;
using BLL.Services.UserCarrerPathServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserCareerPathController : ControllerBase
    {
        private readonly IUserCareerPathService _userCareerPathService;

        public UserCareerPathController(IUserCareerPathService userCareerPathService)
        {
            _userCareerPathService = userCareerPathService;
        }

        private string? GetUserId()
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

        [HttpGet("my-career-paths")]
        public async Task<IActionResult> GetMyCareerPaths()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.GetUserCareerPathsAsync(userId));
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCareerPaths()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.GetActiveCareerPathsAsync(userId));
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedCareerPaths()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.GetCompletedCareerPathsAsync(userId));
        }

        [HttpGet("{userCareerPathId:int}")]
        public async Task<IActionResult> GetById(int userCareerPathId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.GetUserCareerPathByIdAsync(userId, userCareerPathId));
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] UserCareerPathRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.EnrollInCareerPathAsync(userId, request));
        }

        [HttpPut("update-status/{userCareerPathId:int}")]
        public async Task<IActionResult> UpdateStatus(int userCareerPathId, [FromBody] UserCareerPathRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.UpdateCareerPathStatusAsync(userId, userCareerPathId, request));
        }

        [HttpPost("complete/{userCareerPathId:int}")]
        public async Task<IActionResult> Complete(int userCareerPathId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.CompleteCareerPathAsync(userId, userCareerPathId));
        }

        [HttpDelete("unenroll/{userCareerPathId:int}")]
        public async Task<IActionResult> Unenroll(int userCareerPathId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.UnenrollFromCareerPathAsync(userId, userCareerPathId));
        }

        [HttpGet("is-enrolled/{careerPathId:int}")]
        public async Task<IActionResult> IsEnrolled(int careerPathId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.IsUserEnrolledAsync(userId, careerPathId));
        }

        [HttpGet("progress/{careerPathId:int}")]
        public async Task<IActionResult> GetProgress(int careerPathId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.GetCareerPathProgressAsync(userId, careerPathId));
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommended()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            return HandleResult(await _userCareerPathService.GetRecommendedCareerPathsAsync(userId));
        }
    }
}

