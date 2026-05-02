using Microsoft.AspNetCore.Mvc;
using BLL.Dtos.CareerPathDtos;
using BLL.Common;
using BLL.Services.CareerPathServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CareerPathController : ControllerBase
    {
        private readonly ICareerPathService _careerPathService;

        public CareerPathController(ICareerPathService careerPathService)
        {
            _careerPathService = careerPathService;
        }

        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
                return Ok(new { Data = result.Data, Message = result.Data as string });

            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                ServiceErrorCode.ValidationError => BadRequest(new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _careerPathService.GetAllCareerPathsAsync();

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses([FromQuery] string name)
        {
            var userId = User.FindFirstValue("uid");

            var result = await _careerPathService.SearchCareerPathsAsync(name, userId);

            if (result.IsSuccess) return Ok(result.Data);

            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _careerPathService.GetCareerPathByIdAsync(id);

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpPost("add new careerPath")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CareerPathRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _careerPathService.CreateCareerPathAsync(request);

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }
        [HttpPut("updatecareerPath/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCareerPathRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _careerPathService.UpdateCareerPathAsync(id, request);

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }
        [HttpDelete("deletecareerPath/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _careerPathService.DeleteCareerPathAsync(id);

            if (result.IsSuccess) return Ok(new { Message = result.Data });
            return HandleResult(result);
        }
    }
}
