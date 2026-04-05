using BLL.Dtos.SkillDtos;
using BLL.Services.SkillService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;
        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue("uid");
        }

        [AllowAnonymous]
        [HttpGet("global-skills")]
        public async Task<IActionResult> GetAllSkills()
        {
            var result = await _skillService.GetAllGlobalSkillsAsync();

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-global-skill")]
        public async Task<IActionResult> CreateSkill([FromBody] CreateSkillRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _skillService.CreateGlobalSkillAsync(request);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("my-skills")]
        public async Task<IActionResult> GetMySkills()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _skillService.GetUserSkillsAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpPost("add-my-skill")]
        public async Task<IActionResult> AddUserSkill([FromBody] AddUserSkillRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _skillService.AddSkillToUserAsync(userId, request);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }

        [HttpDelete("remove-my-skill/{userSkillId}")]
        public async Task<IActionResult> RemoveUserSkill(int userSkillId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _skillService.RemoveUserSkillAsync(userId, userSkillId);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }
    }
}
