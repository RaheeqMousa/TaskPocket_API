using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TaskPocket.DAL.DTO.Requests;
using TaskPocket.DAL.DTO.Responses;

namespace TaskPocketAPI.PL.Area.Identity
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountController : ControllerBase
    {
        private readonly TaskPocket.BLL.Services.Interfaces.IAthenticationService authenticationService;

        public AccountController(TaskPocket.BLL.Services.Interfaces.IAthenticationService authService)
        {
            this.authenticationService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> LoginAsync([FromBody] TaskPocket.DAL.DTO.Requests.LoginRequest loginRequest)
        {
            var userResponse = await authenticationService.LoginAsync(loginRequest);
            if (userResponse == null)
                return Unauthorized("Invalid username or password");

            return Ok(userResponse);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> RegisterAsync([FromBody] TaskPocket.DAL.DTO.Requests.RegisterRequest registerRequest) // Ensure the correct RegisterRequest type is used
        {
            var userResponse = await authenticationService.RegisterAsync(registerRequest, Request);
            return Ok(userResponse);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult<string>> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            var userResponse = await authenticationService.ConfirmEmail(token, userId);
            return Ok(userResponse);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<string>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await authenticationService.ForgotPassword(request);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<string>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await authenticationService.ResetPassword(request);
            if (result)
            {
                return Ok("Password reset successfully.");
            }
            else
            {
                return BadRequest("Failed to reset password. Please check your code and try again.");
            }
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<UserResponse>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var response = await authenticationService.LoginWithOtpAsync(request.Email, request.Otp);
            return Ok(response);
        }

    }
}
