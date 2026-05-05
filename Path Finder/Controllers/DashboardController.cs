using BLL.Services.DashbordServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IUserDashboardService _dashboardService;

        public DashboardController(IUserDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        private string? GetUserId()
        {
            return User.FindFirstValue("uid")
                   ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _dashboardService.GetUserDashboardAsync(userId);

            return Ok(result);
        }
    }
}
