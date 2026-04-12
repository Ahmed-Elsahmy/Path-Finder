using Microsoft.AspNetCore.Mvc;
using BLL.Dtos.CareerPathDtos;
using BLL.Services.CareerPathServices;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareerPathController : ControllerBase
    {
        private readonly ICareerPathService _careerPathService;

        public CareerPathController(ICareerPathService careerPathService)
        {
            _careerPathService = careerPathService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _careerPathService.GetAllCareerPathsAsync();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _careerPathService.GetCareerPathByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CareerPathRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _careerPathService.CreateCareerPathAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCareerPathRQ request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _careerPathService.UpdateCareerPathAsync(id, request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _careerPathService.DeleteCareerPathAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}