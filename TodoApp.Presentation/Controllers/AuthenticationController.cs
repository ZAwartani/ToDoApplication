using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Presentation.BackgroundJobs;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.BackgroundJob;

namespace TodoApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentications _authentications;
        private readonly IAuthenticationBackgroundJob _authenticationBackgroundJob;

        public AuthenticationController(IAuthentications authentications , IAuthenticationBackgroundJob authenticationBackgroundJob)
        {
            _authentications = authentications;
            _authenticationBackgroundJob = authenticationBackgroundJob;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authentications.Login(dto);

            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterNewUserDto dto)
        {
            await _authentications.RegisterNewUserAsync(dto);

            return Ok("Register Successful");
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(SendOTP dto)
        {
            await _authenticationBackgroundJob.SendCodeAsync(dto);
            return Ok("We will send you a code to reset your password.");
        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ForgetPasswordOTP dto)
        {
            var result = await _authentications.ChangePassword(dto);

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshToken dto)
        {
            var result = await _authentications.RefreshTokenAsync(dto);

            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(LogoutDto dto)
        {
            await _authentications.Logout(dto);

            return Ok(new
            {
                message = "Logout Successful"
            });
        }
    }
}