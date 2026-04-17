using BLL.Dtos.AuthDtos;
using BLL.Services.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRQ model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Confirm-Email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRQ model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ConfirmEmailAsync(model);

            // If the message is not success, return BadRequest
            if (result.Message != "Email confirmed successfully! You can now log in." && result.Message != "Email is already confirmed. You can log in directly.")
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRQ model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Google-login")]
        public async Task<IActionResult> GoogleSignInAsync([FromBody] GoogleAuthRQ model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GoogleSignInAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Forgot-Password")]

        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRQ model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(model);

            return Ok(result); // Return 200 OK so the frontend knows the request succeeded
        }
        [HttpPost("Reset-Password")]

        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRQ model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(model);

            if (result.Message != "Password has been reset successfully!")
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOTPRQ model)
        {
            var result = await _authService.ResendOtpAsync(model);
            return Ok(result);
        }

    }
}
