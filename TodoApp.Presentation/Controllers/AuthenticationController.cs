using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;

namespace TodoApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentications _authentications;
        public AuthenticationController(IAuthentications authentications)
        {
            this._authentications = authentications;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authentications.Login(dto);
            if (result != null) {
                return Ok(new
                {
                    message = "Login Sucessful",
                    token = result.AccessToken,
                    RefreshToken = result.RefreshToken
                });
            }
            else
            {
                return Unauthorized(new {Message = "The Email Or Password Is Uncorrect"});
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterNewUserDto dto)
        {
            await _authentications.RegisterNewUserAsync(dto);
            return Ok("Register Sucessful");
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody]SendOTP sendOTP)
        {
            var isSent = await _authentications.SendCodeAsync(sendOTP);
            if (!isSent)
            {
                return BadRequest(new {message = "The Email Not Found!"});
            }
            else
            {
                return Ok(new {message = "Send Code Sucessful"});
            }
        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ForgetPasswordOTP dto)
        {
            var result = await _authentications.ChangePassword(dto);
            if(result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> FreshToken(RefreshToken dto)
        {
            var token = await _authentications.RefreshTokenAsync(dto);
            if (token != null)
            {
                return Ok(new
                {
                    message = "Refresh Token Sucessfull",
                    token = token.AccessToken,
                    Refresh = token.RefreshToken
                });
            }
            else
            {
                return Unauthorized(new {message = "The Code Is Not Correct"});
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(LogoutDto dto)
        {
            var isloggedout = await _authentications.Logout(dto);
            if (isloggedout)
            {
                return Ok("Logout");
            }
            else
            {
                return BadRequest("Logout Un-Sucessful");
            }
        }
    }
}
