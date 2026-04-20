using BLL.Services.SavedJobServices;
using BLL.Dtos.JobDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/saved-jobs")]
    [ApiController]
    [Authorize]
    public class SavedJobController : ControllerBase
    {
        private readonly ISavedJobService _service;

        public SavedJobController(ISavedJobService service)
        {
            _service = service;
        }

        private string? GetUserId() => User.FindFirstValue("uid");

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveJobRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.SaveJobAsync(userId, request);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet]
        public async Task<IActionResult> GetSaved()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetSavedJobsAsync(userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.RemoveSavedJobAsync(userId, id);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet("{jobId}/check")]
        public async Task<IActionResult> IsSaved(int jobId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.IsSavedAsync(userId, jobId);
            return result.IsSuccess ? Ok(new { IsSaved = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }
    }
}
