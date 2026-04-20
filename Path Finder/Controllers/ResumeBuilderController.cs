using BLL.Common;
using BLL.Dtos.AiDtos;
using BLL.Services.ResumeBuilderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResumeBuilderController : ControllerBase
    {
        private readonly IResumeBuilderService _resumeBuilderService;

        public ResumeBuilderController(IResumeBuilderService resumeBuilderService)
        {
            _resumeBuilderService = resumeBuilderService;
        }

        private string? GetUserId() => User.FindFirst("uid")?.Value;
        [HttpPost("generate-pdf")]
        public async Task<IActionResult> GenerateResumePdf(
                    [FromBody] ResumeBuilderRQ request,
                    CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _resumeBuilderService.GenerateResumePdfAsync(userId, request, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ServiceErrorCode.ValidationError => BadRequest(new { Message = result.ErrorMessage }),
                    ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                    _ => BadRequest(new { Message = result.ErrorMessage })
                };
            }

            var pdfBytes = result.Data.PdfBytes;
            var fullName = result.Data.FullName;

            var safeFullName = string.Join("_", fullName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeFullName} Resume - Path Finder.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}