using BLL.Common;
using BLL.Dtos.NotificationDtos;
using BLL.Services.NotificationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Path_Finder.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        private string? GetUserId() => User.FindFirstValue("uid");

        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Data);

            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                ServiceErrorCode.Unauthorized => Unauthorized(new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] bool unreadOnly = false, [FromQuery] int take = 50)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetUserNotificationsAsync(userId, unreadOnly, take);
            return HandleResult(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.GetUnreadCountAsync(userId);
            return HandleResult(result);
        }

        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.MarkAsReadAsync(userId, id);
            return HandleResult(result);
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.MarkAllAsReadAsync(userId);
            if (result.IsSuccess) return Ok(new { Message = result.Data });
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRQ request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.CreateAsync(request);
            return HandleResult(result);
        }
    }
}

