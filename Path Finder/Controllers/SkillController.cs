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
            // Extracts the User ID from the currently logged in user's Token
            var userIdClaim = User.FindFirstValue("uid");
            return userIdClaim;
        }
        [AllowAnonymous] // Anyone can see the global list of skills (dropdowns)
        [HttpGet("global-skills")]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _skillService.GetAllGlobalSkillsAsync();
            return Ok(skills);
        }
        [Authorize(Roles = "Admin")] // Only Admins can create new global skills
        [HttpPost("create-global-skill")]
        public async Task<IActionResult> CreateSkill([FromBody] CreateSkillRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _skillService.CreateGlobalSkillAsync(request);
            if (result == "Skill created successfully.") return Ok(new { Message = result });
            return BadRequest(result);
        }
        [Authorize(Roles = "User,Admin")] // Both Users and Admins can access their own skills
        [HttpGet("my-skills")]
        public async Task<IActionResult> GetMySkills()
        {
            var userId = GetUserId();
            var mySkills = await _skillService.GetUserSkillsAsync(userId);
            return Ok(mySkills);
        }
        [HttpPost("add-my-skill")]
        public async Task<IActionResult> AddUserSkill([FromBody] AddUserSkillRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _skillService.AddSkillToUserAsync(userId, request);

            if (result == "Skill added to your profile.") return Ok(new { Message = result });
            return BadRequest(result);
        }
        [HttpDelete("remove-my-skill/{userSkillId}")]
        public async Task<IActionResult> RemoveUserSkill(int userSkillId)
        {
            var userId = GetUserId();
            var result = await _skillService.RemoveUserSkillAsync(userId, userSkillId);

            if (result == "Skill removed successfully.") return Ok(new { Message = result });
            return BadRequest(result);
        }
    }
}
