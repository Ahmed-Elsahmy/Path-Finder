using BLL.Dtos.CareerPathCourseDtos;
using BLL.Services.CareerPathCourseServices;
using Microsoft.AspNetCore.Http;
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

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CareerPathCourseRQ request)
        //{
        //    var result = await _service.CreateAsync(request);

        //    if (!result.IsSuccess)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        [HttpGet("career-path/{careerPathId}")]
        public async Task<IActionResult> GetByCareerPathId(int careerPathId)
        {
            var result = await _service.GetByCareerPathIdAsync(careerPathId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await _service.DeleteAsync(id);

        //    if (!result.IsSuccess)
        //        return NotFound(result);

        //    return Ok(result);
        //}
    }
}
