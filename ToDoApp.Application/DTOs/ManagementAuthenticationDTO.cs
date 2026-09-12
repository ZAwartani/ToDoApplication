namespace ToDoApp.Application.Dtos
{
    public record LoginDto(string Email , string Password);
    public record RegisterNewUserDto(string UserName, string Email, string Password);
    public record SendOTP(string Email);
    public record ForgetPasswordOTP(string Email,string Password,string OTP);
    public record LogoutDto(string Email, string RefreshToken);
    public record TokenRespone(string AccessToken, string RefreshToken);
    public record RefreshToken(string Email, string Refreshtoken);

}
  