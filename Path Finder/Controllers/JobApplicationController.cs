using BLL.Services.JobApplicationServices;
using BLL.Dtos.JobDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/job-applications")]
    [ApiController]
    [Authorize]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _service;

        public JobApplicationController(IJobApplicationService service)
        {
            _service = service;
        }

        private string? GetUserId() => User.FindFirstValue("uid");

        [HttpPost]
        public async Task<IActionResult> Apply([FromBody] ApplyJobRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.ApplyToJobAsync(userId, request);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyApplications()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetUserApplicationsAsync(userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateApplicationRQ request)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.UpdateApplicationAsync(userId, id, request);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.WithdrawApplicationAsync(userId, id);
            return result.IsSuccess ? Ok(new { Message = result.Data }) : BadRequest(new { Message = result.ErrorMessage });
        }
    }
}
