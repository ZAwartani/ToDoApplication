using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ToDoApp.Application.DTOs
{
    public record LoginDto(string Email , string Password);
    public record RegisterNewUserDto(string UserName, string Email, string Password);
    public record SendOTP(string email);
    public record ForgetPasswordOTP(string email,string Password,string OTP);
    public record LogoutDto(string Email, string RefreshToken);
    public record TokenRespone(string AccessToken, string RefreshToken);
    public record RefreshToken(string Email, string Refreshtoken);

}
