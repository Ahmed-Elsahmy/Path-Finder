using System.Security.Claims;
using BLL.Common;
using BLL.Dtos.QuestionnaireDtos;
using BLL.Services.QuestionnaireServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionnaireController : ControllerBase
    {
        private readonly IQuestionnaireService _questionnaireService;

        public QuestionnaireController(IQuestionnaireService questionnaireService)
        {
            _questionnaireService = questionnaireService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue("uid")
                   ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ServiceErrorCode.Unauthorized => Unauthorized(new { Message = result.ErrorMessage }),
                ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }

        [HttpGet("career-match")]
        public async Task<IActionResult> GetCareerMatch(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _questionnaireService.GetCareerAssessmentAsync(userId, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost("career-match/submit")]
        public async Task<IActionResult> SubmitCareerMatch(
            [FromBody] SubmitCareerAssessmentRQ request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _questionnaireService.SubmitCareerAssessmentAsync(
                userId,
                request,
                cancellationToken);

            return HandleResult(result);
        }
    }
}
