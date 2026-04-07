using BLL.Dtos.UserExperienceDtos;
using BLL.Services.UserExperienceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserExperienceController : ControllerBase
    {
        private readonly IUserExperienceService _experienceService;

        public UserExperienceController(IUserExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        private string GetUserId()
        {
            return User.FindFirst("uid")?.Value;
        }
        [HttpGet("my-experiences")]
        public async Task<IActionResult> GetMyExperiences()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _experienceService.GetUserExperiencesAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("add-experience")]
        public async Task<IActionResult> AddExperience([FromBody] UserExperienceRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _experienceService.AddExperienceAsync(userId, request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPut("update/{experienceId}")]
        public async Task<IActionResult> UpdateExperience(
            int experienceId,
            [FromBody] UpdateUserExperienceRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _experienceService
                .UpdateUserProfileAsync(userId, experienceId, request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("delete/{experienceId}")]
        public async Task<IActionResult> DeleteExperience(int experienceId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _experienceService
                .DeleteExperienceAsync(userId, experienceId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
