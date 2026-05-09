using BLL.Dtos.RecentSearchDtos;
using BLL.Services.RecentSearchServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecentSearchController : ControllerBase
    {
        private readonly IRecentSearchService _service;

        public RecentSearchController(IRecentSearchService service)
        {
            _service = service;
        }

        private string? GetUserId() => User.FindFirstValue("uid");

        [HttpGet("courses")]
        public async Task<IActionResult> GetRecnetCourses()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetRecentCoursesAsync(userId);

            return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(new { Message = result.ErrorMessage });
        }
        [HttpGet("jobs")]
        public async Task<IActionResult> GetRecnetJobs()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetRecentJobsAsync(userId);

            return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(new { Message = result.ErrorMessage });
        }
        [HttpGet("careerpaths")]
        public async Task<IActionResult> GetRecnetCareerPaths()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetRecentCareerPathsAsync(userId);

            return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(new { Message = result.ErrorMessage });
        }
        [HttpDelete("courses")]
        public async Task<IActionResult> ClearRecentCourses()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();
            var result = await _service.ClearRecentCoursesAsync(userId);
            return result.IsSuccess
                ? Ok(new { Message = result.Data })
                : BadRequest(new { Message = result.ErrorMessage });
        }
        [HttpDelete("jobs")]
        public async Task<IActionResult> ClearRecentJobs()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();
            var result = await _service.ClearRecentJobsAsync(userId);
            return result.IsSuccess
                ? Ok(new { Message = result.Data })
                : BadRequest(new { Message = result.ErrorMessage });
        }
        [HttpDelete("careerpaths")]
        public async Task<IActionResult> ClearRecentCareerPaths()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();
            var result = await _service.ClearRecentCareerPathsAsync(userId);
            return result.IsSuccess
                ? Ok(new { Message = result.Data })
                : BadRequest(new { Message = result.ErrorMessage });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> ClearRecentSearchById(int id)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();
            var result = await _service.ClearRecentSearchByIdAsync(userId, id);
            return result.IsSuccess
                ? Ok(new { Message = result.Data })
                : BadRequest(new { Message = result.ErrorMessage });
        }
    }
}
