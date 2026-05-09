using System.Security.Claims;
using BLL.Dtos.SavedCourseDtos;
using BLL.Services.SavedCourseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SavedCourseController : ControllerBase
    {
        private readonly ISavedCourseService _service;

        public SavedCourseController(ISavedCourseService service)
        {
            _service = service;
        }

        private string? GetUserId() => User.FindFirstValue("uid");

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SavedCourseRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.SaveCourseAsync(userId, request);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet]
        public async Task<IActionResult> GetSaved()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetSavedCoursesAsync(userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.RemoveSavedCourseAsync(userId, id);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet("{courseId}/check")]
        public async Task<IActionResult> IsSaved(int courseId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.IsSavedAsync(userId, courseId);
            return result.IsSuccess ? Ok(new { IsSaved = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }
    }
}
