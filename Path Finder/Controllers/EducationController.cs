using BLL.Dtos.EducationDtos;
using BLL.Services.EducationService;
using BLL.Services.EducationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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
            var educationList = await _educationService.GetUserEducationAsync(userId);
            return Ok(educationList);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddEducation([FromForm] EducationRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _educationService.AddEducationAsync(userId, request);

            if (result == "Education added successfully.")
                return Ok(new { Message = result });

            return BadRequest(new { Message = result });
        }

        [HttpPut("update/{educationId}")]
        public async Task<IActionResult> UpdateEducation(int educationId, [FromForm] EducationRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _educationService.UpdateEducationAsync(userId, educationId, request);

            if (result == "Education updated successfully.")
                return Ok(new { Message = result });

            return BadRequest(new { Message = result });
        }

        [HttpDelete("delete/{educationId}")]
        public async Task<IActionResult> DeleteEducation(int educationId)
        {
            var userId = GetUserId();
            var result = await _educationService.DeleteEducationAsync(userId, educationId);

            if (result == "Education deleted successfully.")
                return Ok(new { Message = result });

            return BadRequest(new { Message = result });
        }
        [HttpPost("{educationId}/upload-certificates")]
        public async Task<IActionResult> UploadCertificates(int educationId, [FromForm] List<IFormFile> files)
        {
            var userId = GetUserId();
            var result = await _educationService.UploadCertificateAsync(userId, educationId, files);

            if (result == "Certificates uploaded successfully.")
                return Ok(new { Message = result });

            return BadRequest(new { Message = result });
        }

        [HttpDelete("{educationId}/delete-specific-certificate")]
        public async Task<IActionResult> DeleteSpecificCertificate(int educationId, [FromQuery] string certificateUrl)
        {
            var userId = GetUserId();

            // We get the URL from the query string (e.g., ?certificateUrl=/Uploads/...)
            var result = await _educationService.DeleteCertificateAsync(userId, educationId, certificateUrl);

            if (result == "Certificate deleted successfully.")
                return Ok(new { Message = result });

            return BadRequest(new { Message = result });
        }
    }
}