using BLL.Services.JobServices;
using BLL.Dtos.JobDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    [Authorize]
    public class JobController : ControllerBase
    {
        private readonly IJobService _service;

        public JobController(IJobService service)
        {
            _service = service;
        }

        private string? GetUserId() => User.FindFirstValue("uid");

        [HttpGet]
        public async Task<IActionResult> GetJobs([FromQuery] JobFilterRQ filter)
        {
            var result = await _service.GetJobsAsync(filter);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetJobByIdAsync(id);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] JobRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateJobAsync(request);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobRQ request)
        {
            var result = await _service.UpdateJobAsync(id, request);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteJobAsync(id);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommended()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetRecommendedJobsAsync(userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }
    }
}
