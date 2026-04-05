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
        public CvController(ICvService cvService)
        {
            _cvService = cvService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue("uid");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCv([FromForm] UploadCvRQ request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("File is empty.");

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await _cvService.UploadCvAsync(userId, request, baseUrl);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }

        [HttpGet("my-cvs")]
        public async Task<IActionResult> GetMyCvs()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _cvService.GetUserCvsAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpPut("{cvId}/set-primary")]
        public async Task<IActionResult> SetPrimaryCv(int cvId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _cvService.SetPrimaryCvAsync(userId, cvId);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }

        [HttpDelete("{cvId}")]
        public async Task<IActionResult> DeleteCv(int cvId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _cvService.DeleteCvAsync(userId, cvId);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = result.Data });
        }
        [HttpPost("compare")]
        public async Task<IActionResult> CompareCvs(
            [FromBody] CvComparisonRQ request,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _cvService.CompareCvsAsync(userId, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.ErrorMessage }); 

            return Ok(result.Data);
        }
    }
}
