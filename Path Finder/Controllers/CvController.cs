using BLL.Dtos.CvDtos;
using BLL.Services.CvService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CvController : ControllerBase
    {
        private readonly ICvService _cvService;
        public CvController(ICvService cvService) {
            _cvService = cvService;
        }
        private string? GetUserId()
        {
            // Extracts the User ID from the currently logged in user's Token
            var userIdClaim = User.FindFirstValue("uid");
            return userIdClaim;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCv([FromForm] UploadCvRQ request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("File is empty.");

            var userId = GetUserId();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var result = await _cvService.UploadCvAsync(userId, request, baseUrl);

            if (result == "CV Uploaded Successfully")
                return Ok(new { Message = result });

            return BadRequest(result);
        }

        [HttpGet("my-cvs")]
        public async Task<IActionResult> GetMyCvs()
        {
            var userId = GetUserId();
            var cvs = await _cvService.GetUserCvsAsync(userId);
            return Ok(cvs);
        }
        [HttpPut("{cvId}/set-primary")]
        public async Task<IActionResult> SetPrimaryCv(int cvId)
        {
            var userId = GetUserId();
            var result = await _cvService.SetPrimaryCvAsync(userId, cvId);

            if (result == "Primary CV updated.")
                return Ok(new { Message = result });

            return BadRequest(result);
        }

        [HttpDelete("{cvId}")]
        public async Task<IActionResult> DeleteCv(int cvId)
        {
            var userId = GetUserId();
            var result = await _cvService.DeleteCvAsync(userId, cvId);

            if (result == "CV Deleted Successfully.")
                return Ok(new { Message = result });

            return BadRequest(result);
        }
    }
}
