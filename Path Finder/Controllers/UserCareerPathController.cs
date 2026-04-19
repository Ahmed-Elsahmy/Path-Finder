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

            var result = await _userCareerPathService.GetUserCareerPathsAsync(userId);
            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpGet("GetCareerPaths")]
        public async Task<IActionResult> GetCareerPaths([FromQuery] UserCareerPathFilter filter)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _userCareerPathService.GetCareerPathsAsync(userId, filter);
            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }


        [HttpGet("{userCareerPathId:int}")]
        public async Task<IActionResult> GetById(int userCareerPathId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _userCareerPathService.GetUserCareerPathByIdAsync(userId, userCareerPathId);
            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] UserCareerPathRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _userCareerPathService.EnrollInCareerPathAsync(userId, request);
            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpDelete("unenroll/{userCareerPathId:int}")]
        public async Task<IActionResult> Unenroll(int userCareerPathId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _userCareerPathService.UnenrollFromCareerPathAsync(userId, userCareerPathId);
            if (result.IsSuccess) return Ok(new { Message = result.Data });
            return HandleResult(result);
        }

        [HttpGet("is-enrolled/{careerPathId:int}")]
        public async Task<IActionResult> IsEnrolled(int careerPathId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _userCareerPathService.IsUserEnrolledAsync(userId, careerPathId);
            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommended()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _userCareerPathService.GetRecommendedCareerPathsAsync(userId);
            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }
    }
}
