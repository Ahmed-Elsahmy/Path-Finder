using BLL.Common;
using BLL.Dtos.CoursePlatformDtos;
using BLL.Services.CoursePlatformService;
using BLL.Services.CoursePlatformServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursePlatformController : ControllerBase
    {
        private readonly ICoursePlatformService _platformService;

        public CoursePlatformController(ICoursePlatformService platformService)
        {
            _platformService = platformService;
        }
        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(new { Message = result.Data });
            }

            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                ServiceErrorCode.ValidationError => BadRequest(new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }
        [HttpGet("Get-All-Platforms")]
        public async Task<IActionResult> GetAllPlatforms([FromQuery] bool onlyActive = true)
        {
            var result = await _platformService.GetAllPlatformsAsync(onlyActive);

            if (result.IsSuccess)
                return Ok(result.Data);

            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlatformById(int id)
        {
            var result = await _platformService.GetPlatformByIdAsync(id);

            if (result.IsSuccess)
                return Ok(result.Data);

            return HandleResult(result);
        }
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePlatform([FromBody] CoursePlatformRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _platformService.CreatePlatformAsync(request);

            return HandleResult(result);
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePlatform(int id, [FromForm] UpdateCoursePlatformRQ request)
        {
            var form = Request.Form;

            var result = await _platformService.UpdatePlatformAsync(id, request, form);

            return HandleResult(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlatform(int id)
        {
            var result = await _platformService.DeletePlatformAsync(id);

            return HandleResult(result);
        }
    }
}