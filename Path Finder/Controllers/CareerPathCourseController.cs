using BLL.Dtos.CareerPathCourseDtos;
using BLL.Services.CareerPathCourseServices;
using BLL.Common;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareerPathCourseController : ControllerBase
    {
        private readonly ICareerPathCourseService _service;

        public CareerPathCourseController(ICareerPathCourseService service)
        {
            _service = service;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CareerPathCourseRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.CreateAsync(request);

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpGet("career-path/{careerPathId}")]
        public async Task<IActionResult> GetByCareerPathId(int careerPathId)
        {
            var result = await _service.GetByCareerPathIdAsync(careerPathId);

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (result.IsSuccess) return Ok(new { Message = result.Data });
            return HandleResult(result);
        }
    }
}
