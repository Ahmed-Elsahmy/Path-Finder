using BLL.Dtos.UserProfileDtos;
using BLL.Services.UserProfileServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        private string GetUserId()
        {
            return User.FindFirst("uid")?.Value;
        }
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _userProfileService.GetUserProfileAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(result.Data);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateUserProfileRQ request)
        {
            var userId = GetUserId();

            var result = await _userProfileService.UpdateUserProfileAsync(
                userId,
                request,
                Request.Form);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
