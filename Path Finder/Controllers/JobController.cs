using BLL.Services.JobServices;
using BLL.Dtos.JobDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BLL.Common;

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
        [HttpGet("search")]
        public async Task<IActionResult> SearchJobs([FromQuery] string name)
        {
            var userId = User.FindFirstValue("uid");

            var result = await _service.SearchJobsAsync(name, userId);

            if (result.IsSuccess) return Ok(result.Data);

            return HandleResult(result);
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
