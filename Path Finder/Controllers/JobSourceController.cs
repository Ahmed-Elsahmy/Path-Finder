using BLL.Services.JobSourceServices;
using BLL.Dtos.JobDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/job-sources")]
    [ApiController]
    [Authorize]
    public class JobSourceController : ControllerBase
    {
        private readonly IJobSourceService _service;

        public JobSourceController(IJobSourceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = true)
        {
            var result = await _service.GetAllSourcesAsync(onlyActive);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetSourceByIdAsync(id);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] JobSourceRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateSourceAsync(request);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobSourceRQ request)
        {
            var result = await _service.UpdateSourceAsync(id, request);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteSourceAsync(id);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }
    }
}
