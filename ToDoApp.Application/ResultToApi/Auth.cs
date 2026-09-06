using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Services;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.ResultToApi
{
    public class Auth
    {
        private readonly AuthenticationServices _AuthenticationServices;

        public Auth(AuthenticationServices authenticationServices)
        {
            _AuthenticationServices = authenticationServices;
        }

        public async Task<object> Login(LoginDto dto)
        {
            var result = await _AuthenticationServices.Login(dto);

            if (result != null)
                return new {Message = "Login" , token = result.AccessToken , RefreshToken = result.RefreshToken};

            return false;
        }

        public async Task<bool> Register(RegisterNewUserDto dto)
        {
            await _AuthenticationServices.RegisterNewUserAsync(dto);

            return true;
        }

        public async Task<bool> ForgetPassword(SendOTP dto)
        {
            await _AuthenticationServices.SendCodeAsync(dto);

            return true;
        }

        public async Task<bool> ResetPassword(ForgetPasswordOTP dto)
        {
            var result = await _AuthenticationServices.ChangePassword(dto);

            if (result != null)
                return true;

            return false;
        }

        public async Task<object> RefreshToken(RefreshToken dto)
        {
            var result = await _AuthenticationServices.RefreshTokenAsync(dto);

            if (result != null)
                return new
                {
                    Message = "New Token With Refresh",
                    token = result.AccessToken,
                    RefreshToken = result.RefreshToken
                };

            return false;
        }

        public async Task<bool> Logout(LogoutDto dto)
        {
            await _AuthenticationServices.Logout(dto);

            return true;
        }
    }
}