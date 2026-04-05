using BLL.Dtos.EducationDtos;
using BLL.Services.EducationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EducationController : ControllerBase
    {
        private readonly IEducationService _educationService;

        public EducationController(IEducationService educationService)
        {
            _educationService = educationService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue("uid");
        }

        [HttpGet("my-education")]
        public async Task<IActionResult> GetMyEducation()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _educationService.GetUserEducationAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddEducation([FromForm] EducationRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _educationService.AddEducationAsync(userId, request);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }

        [HttpPatch("update/{educationId}")]
        public async Task<IActionResult> UpdateEducation(int educationId, [FromBody] JsonPatchDocument<UpdateEducationRQ> patchDoc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _educationService.UpdateEducationAsync(userId, educationId, patchDoc);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }

        [HttpDelete("delete/{educationId}")]
        public async Task<IActionResult> DeleteEducation(int educationId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _educationService.DeleteEducationAsync(userId, educationId);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }

        [HttpPost("{educationId}/upload-certificates")]
        public async Task<IActionResult> UploadCertificates(int educationId, [FromForm] List<IFormFile> files)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _educationService.UploadCertificateAsync(userId, educationId, files);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }

        [HttpDelete("{educationId}/delete-specific-certificate")]
        public async Task<IActionResult> DeleteSpecificCertificate(int educationId, [FromQuery] string certificateUrl)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _educationService.DeleteCertificateAsync(userId, educationId, certificateUrl);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }
    }
}