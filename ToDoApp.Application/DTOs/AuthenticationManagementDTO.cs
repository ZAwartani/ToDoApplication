namespace ToDoApp.Application.Dtos
{
    public record LoginDto(string Email , string Password);
    public record RegisterNewUserDto(string UserName, string Email, string Password);
    public record SendOTPDto(string Email);
    public record ForgetPasswordOTPDto(string Email,string Password,string OTP);
    public record LogoutDto(string Email, string RefreshToken);
    public record TokenResponeDto(string AccessToken, string RefreshToken);
    public record RefreshTokenDto(string Email, string Refreshtoken);

}
  