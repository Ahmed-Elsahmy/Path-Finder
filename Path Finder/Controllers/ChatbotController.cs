using BLL.Common;
using BLL.Dtos.AiDtos;
using BLL.Services.ChatbotService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        private string? GetUserId() => User.FindFirstValue("uid");

        [HttpPost("ask")]
        public async Task<IActionResult> Ask(
            [FromForm] ChatRQ request,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var username = User.FindFirstValue("sub")
                        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("username")
                        ?? "Friend";
            var email = User.FindFirstValue(ClaimTypes.Email)
                     ?? User.FindFirstValue("email");

            var result = await _chatbotService.AskQuestionAsync(
                request, userId, username, email, cancellationToken);

            return HandleResult(result);
        }

        /// <summary>Feature 6 — Generate a career roadmap based on user's skills and target job</summary>
        [HttpPost("career-roadmap")]
        public async Task<IActionResult> CareerRoadmap(
            [FromBody] CareerRoadmapRQ request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _chatbotService.GenerateCareerRoadmapAsync(
                userId, request, cancellationToken);

            return HandleResult(result);
        }

        /// <summary>Feature 8 — Generate mock interview questions for a given role</summary>
        [HttpPost("interview-prep")]
        public async Task<IActionResult> InterviewPrep(
            [FromBody] InterviewPrepRQ request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _chatbotService.GenerateInterviewPrepAsync(
                userId, request, cancellationToken);

            return HandleResult(result);
        }

        /// <summary>Shared result handler — returns correct HTTP status based on error type</summary>
        private IActionResult HandleResult(ServiceResult<string> result)
        {
            if (!result.IsSuccess)
            {
                if (result.ErrorCode == ServiceErrorCode.UpstreamServiceError)
                    return StatusCode(503, new { Message = result.ErrorMessage });

                return BadRequest(new { Message = result.ErrorMessage });
            }

            return Ok(new { Reply = result.Data });
        }
    }
}
