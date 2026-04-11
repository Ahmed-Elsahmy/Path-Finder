using BLL.Common;
using BLL.Dtos.CourseDtos;
using BLL.Services.CourseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
                return Ok(new { Data = result.Data });

            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                ServiceErrorCode.ValidationError => BadRequest(new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCourses([FromQuery] CourseFilterRQ filter)
        {
            var result = await _courseService.GetCoursesAsync(filter);

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var result = await _courseService.GetCourseByIdAsync(id);

            if (result.IsSuccess) return Ok(result.Data);
            return HandleResult(result);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourse([FromForm] CourseRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _courseService.CreateCourseAsync(request);
            if (result.IsSuccess) return Ok(new { Message = result.Data });

            return HandleResult(result);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse(int id, [FromForm] UpdateCourseRQ request)
        {
            var form = Request.Form;
            var result = await _courseService.UpdateCourseAsync(id, request, form);

            if (result.IsSuccess) return Ok(new { Message = result.Data });

            return HandleResult(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);

            if (result.IsSuccess) return Ok(new { Message = result.Data });

            return HandleResult(result);
        }
    }
}