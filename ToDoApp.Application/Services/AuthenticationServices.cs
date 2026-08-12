using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services
{
    public class AuthenticationServices : IAuthentications
    {
        private readonly IGenericRepository<Users, int> _genericRepositoryUser;
        private readonly IGenericRepository<Token, int> _genericRepositoryToken;
        private readonly IGenericRepository<CodeOTP, int> _genericRepositoryCode;
        private readonly ISpecificationBuilderFactory _factory;
        private readonly IConfiguration _configuration;

        public AuthenticationServices(
            IGenericRepository<Users, int> genericRepositoryUser,
            IGenericRepository<Token, int> genericRepositoryToken,
            IGenericRepository<CodeOTP, int> genericRepositoryCode,
            ISpecificationBuilderFactory factory,
            IConfiguration configuration)
        {
            _genericRepositoryUser = genericRepositoryUser;
            _genericRepositoryToken = genericRepositoryToken;
            _genericRepositoryCode = genericRepositoryCode;
            _factory = factory;
            _configuration = configuration;
        }

        public async Task RegisterNewUserAsync(RegisterNewUserDto registerDto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user = new Users(
                registerDto.UserName,
                registerDto.Email,
                passwordHash,
                enumUserRole.User
            );

            await _genericRepositoryUser.AddAsync(user);
        }

        public async Task<TokenRespone?> Login(LoginDto loginDto)
        {
            var spec = _factory.CreateBuilder<Users>()
                .AddCriteria(u => u.Email == loginDto.Email);

            var users = await _genericRepositoryUser.GetAsync(spec);

            if (users == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, users.Password))
                throw new Exception("Invalid email or password");


            var tokenSpec = _factory.CreateBuilder<Token>()
                 .AddCriteria(t => t.UserId == users.Id);

            var userToken = await _genericRepositoryToken.GetAsync(tokenSpec);
            if (userToken != null)
            {
                userToken.RefreshTokenRevokedAt = DateTime.UtcNow;
                await _genericRepositoryToken.UpdateAsync(userToken);
            }

            var tokenResponse = await GenerateAndSaveTokensAsync(users);
            return tokenResponse;
        }

        public async Task Logout(LogoutDto logoutDto)
        {
            var spec = _factory.CreateBuilder<Users>()
                .AddCriteria(u => u.Email == logoutDto.Email);

            var user = (await _genericRepositoryUser.GetAsync(spec));

            if (user == null)
                throw new Exception("User not found");

            var tokenSpec = _factory.CreateBuilder<Token>()
                .AddCriteria(t => t.UserId == user.Id);

            var userToken = await _genericRepositoryToken.GetAsync(tokenSpec);

            if (userToken == null || userToken.RefreshToken == null)
                throw new Exception("The refresh token is invalid");

            if (!BCrypt.Net.BCrypt.Verify(logoutDto.RefreshToken, userToken.RefreshToken))
                throw new Exception("The refresh token is invalid");

            userToken.RefreshTokenRevokedAt = DateTime.UtcNow;

            await _genericRepositoryToken.UpdateAsync(userToken);
        }

        public async Task<string?> ChangePassword(ForgetPasswordOTP forgetDto)
        {
            var userSpec = _factory.CreateBuilder<Users>()
                .AddCriteria(u => u.Email == forgetDto.email);

            var users = await _genericRepositoryUser.GetAsync(userSpec);

            if (users == null)
                throw new Exception("User not found");

            var otpSpec = _factory.CreateBuilder<CodeOTP>()
                .AddCriteria(c => c.UserId == users.Id);

            var otpList = await _genericRepositoryCode.ListAsync(otpSpec);
            var otp = otpList.SingleOrDefault();

            if (otp == null)
                throw new Exception("Invalid verification code");

            if (otp.Codeotp != forgetDto.OTP)
                throw new Exception("Invalid verification code");

            if (otp.CodeOTPExpir < DateTime.UtcNow)
                throw new Exception("Verification code has expired");

            users.Password = BCrypt.Net.BCrypt.HashPassword(forgetDto.Password);
            await _genericRepositoryUser.UpdateAsync(users);

            await _genericRepositoryCode.DeleteAsync(otp.Id);

            return "Password Changed Successfully";
        }

        public async Task<bool> SendCodeAsync(SendOTP sendOTPDto)
        {
            var spec = _factory.CreateBuilder<Users>()
                .AddCriteria(u => u.Email == sendOTPDto.email);

            var user = (await _genericRepositoryUser.GetAsync(spec));

            if (user == null)
                throw new Exception("User not found");

            string codeOTPToEmail = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("TodoApp", "zakaryawork005@gmail.com"));
            message.To.Add(new MailboxAddress("User", sendOTPDto.email));
            message.Subject = "Reset Your Password - TodoApp";

            message.Body = new TextPart("plain")
            {
                Text = $"This is your verification code: {codeOTPToEmail}. It will expire in 5 minutes."
            };

            await _genericRepositoryCode.RemoveRangeAsync(await _genericRepositoryCode.ListAsync(_factory.CreateBuilder<CodeOTP>().AddCriteria(c => c.UserId == user.Id)));
            
            var otpSpec = _factory.CreateBuilder<CodeOTP>()
                .AddCriteria(c => c.UserId == user.Id);

            var existingOtp =
                (await _genericRepositoryCode.GetAsync(otpSpec));

            if (existingOtp != null)
            {
                existingOtp.Codeotp = codeOTPToEmail;
                existingOtp.CodeOTPExpir = DateTime.UtcNow.AddMinutes(5);

                await _genericRepositoryCode.UpdateAsync(existingOtp);
            }
            else
            {
                var newOtp = new CodeOTP
                (
                    codeOTPToEmail,
                    DateTime.UtcNow.AddMinutes(5),
                    user.Id
                );

                await _genericRepositoryCode.AddAsync(newOtp);
            }

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("zakaryawork005@gmail.com", "qkukqdjeaqbxkjod");
                    await client.SendAsync(message);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"حدث خطأ أثناء الإرسال: {ex.Message}");
                    return false;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }

        public async Task<TokenRespone?> RefreshTokenAsync(RefreshToken refreshToken)
        {
            var spec = _factory.CreateBuilder<Users>()
                .AddCriteria(u => u.Email == refreshToken.Email);

            var user = (await _genericRepositoryUser.GetAsync(spec));

            if (user == null)
                throw new Exception("User not found");

            var tokenSpec = _factory.CreateBuilder<Token>()
                .AddCriteria(t => t.UserId == user.Id);

            var userToken = (await _genericRepositoryToken.GetAsync(tokenSpec));

            if (userToken == null)
                throw new Exception("Invalid refresh token");

            if (userToken.RefreshTokenRevokedAt != null)
                throw new Exception("Refresh token has been revoked");

            if (userToken.RefreshTokenExpir < DateTime.UtcNow)
                throw new Exception("Refresh token has expired");

            if (!BCrypt.Net.BCrypt.Verify(refreshToken.Refreshtoken, userToken.RefreshToken))
                throw new Exception("Invalid refresh token");

            userToken.RefreshTokenRevokedAt = DateTime.UtcNow;

            await _genericRepositoryToken.UpdateAsync(userToken);

            return await GenerateAndSaveTokensAsync(user);
        }

        private async Task<TokenRespone> GenerateAndSaveTokensAsync(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["JWT:Key"] ?? "LONG_SECURE_KEY_32_CHARS_MINIMUM_LENGTH";
            var jwtIssuer = _configuration["JWT:Issuer"];
            var jwtAudience = _configuration["JWT:Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            var accessToken = tokenHandler.WriteToken(jwt);
            var rawRefreshToken = GenerateRefreshToken();

            var userToken = new Token
            {
                UserId = user.Id,
                RefreshToken = BCrypt.Net.BCrypt.HashPassword(rawRefreshToken),
                RefreshTokenExpir = DateTime.UtcNow.AddDays(7),
                RefreshTokenRevokedAt = null
            };
            await _genericRepositoryToken.AddAsync(userToken);

            return new TokenRespone(accessToken, rawRefreshToken);
        }

        private string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}