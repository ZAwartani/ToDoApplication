using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services
{
    public class AuthenticationServices : IAuthentications
    {
        private readonly IGenericRepository<Users, int> _genericRepositoryUser;
        private readonly IGenericRepository<Token, int> _genericRepositoryToken;
        private readonly IGenericRepository<CodeOTP, int> _genericRepositoryCode;
        private readonly IConfiguration _configuration;

        public AuthenticationServices(
            IGenericRepository<Users, int> genericRepositoryUser,
            IGenericRepository<Token, int> genericRepositoryToken,
            IGenericRepository<CodeOTP, int> genericRepositoryCode,
            IConfiguration configuration)
        {
            _genericRepositoryUser = genericRepositoryUser;
            _genericRepositoryToken = genericRepositoryToken;
            _genericRepositoryCode = genericRepositoryCode;
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
            var spec = new GetListByEmail(loginDto.Email);
            var users = await _genericRepositoryUser.ListAsync(spec);
            var user = users.SingleOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                return null;

            var tokenSpec = new GetTokenByIdUser(user.Id);
            var existingTokens = await _genericRepositoryToken.ListAsync(tokenSpec);
            var userToken = existingTokens.SingleOrDefault() ?? new Token{ UserId = user.Id };

            var tokenResponse = await GenerateAndSaveTokensAsync(user, userToken);
            return tokenResponse;
        }

        public async Task<bool> Logout(LogoutDto logoutDto)
        {
            var spec = new GetListByEmail(logoutDto.Email);
            var user = (await _genericRepositoryUser.ListAsync(spec)).SingleOrDefault();

            if (user == null)
                return false;

            var tokenSpec = new GetTokenByIdUser(user.Id);
            var userToken = (await _genericRepositoryToken.ListAsync(tokenSpec)).SingleOrDefault();

            if (userToken == null || userToken.RefreshToken == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(logoutDto.RefreshToken, userToken.RefreshToken))
                return false;

            userToken.RefreshToken = null;
            userToken.RefreshTokenRevokedAt = DateTime.UtcNow;
            userToken.RefreshTokenExpir = null;

            await _genericRepositoryToken.UpdateAsync(userToken);
            return true;
        }

        public async Task<string?> ChangePassword(ForgetPasswordOTP forgetDto)
        {
            var userSpec = new GetListByEmail(forgetDto.email);
            var users = await _genericRepositoryUser.ListAsync(userSpec);
            var user = users.SingleOrDefault();

            if (user == null)
                return null;

            var otpSpec = new GetOTPByUserId(user.Id);
            var otpList = await _genericRepositoryCode.ListAsync(otpSpec);
            var otp = otpList.SingleOrDefault();

            if (otp == null)
                return null;

            if (otp.Codeotp != forgetDto.OTP)
                return null;

            if (otp.CodeOTPExpir < DateTime.UtcNow)
                return null;

            user.Password = BCrypt.Net.BCrypt.HashPassword(forgetDto.HashPassword);
            await _genericRepositoryUser.UpdateAsync(user);

            await _genericRepositoryCode.DeleteAsync(otp.Id);

            return "Password Changed Successfully";
        }

        public async Task<bool> SendCodeAsync(SendOTP sendOTPDto)
        {
            var spec = new GetListByEmail(sendOTPDto.email);

            var user = (await _genericRepositoryUser.ListAsync(spec))
                .SingleOrDefault();

            if (user == null)
                return false;


            string codeOTPToEmail =
                RandomNumberGenerator.GetInt32(10000, 99999).ToString();


            var otpSpec = new GetOTPByUserId(user.Id);

            var existingOtp =
                (await _genericRepositoryCode.ListAsync(otpSpec))
                .SingleOrDefault();


            if (existingOtp != null)
            {
                existingOtp.Codeotp = codeOTPToEmail;
                existingOtp.CodeOTPExpir = DateTime.UtcNow.AddMinutes(5);

                await _genericRepositoryCode.UpdateAsync(existingOtp);
            }
            else
            {
                var newOtp = new CodeOTP
                {
                    Codeotp = codeOTPToEmail,
                    CodeOTPExpir = DateTime.UtcNow.AddMinutes(5),
                    UserId = user.Id
                };

                await _genericRepositoryCode.AddAsync(newOtp);
            }

            return true;
        }

        public async Task<TokenRespone?> RefreshTokenAsync(RefreshToken refreshToken)
        {
            var spec = new GetListByEmail(refreshToken.Email);
            var user = (await _genericRepositoryUser.ListAsync(spec)).SingleOrDefault();

            if (user == null)
                return null;

            var tokenSpec = new GetTokenByIdUser(user.Id);
            var userToken = (await _genericRepositoryToken.ListAsync(tokenSpec)).SingleOrDefault();

            if (userToken == null)
                return null;

            if (userToken.RefreshTokenRevokedAt != null)
                return null;

            if (userToken.RefreshTokenExpir == null || userToken.RefreshTokenExpir < DateTime.UtcNow)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(refreshToken.Refreshtoken, userToken.RefreshToken))
                return null;

            return await GenerateAndSaveTokensAsync(user, userToken);
        }

        private async Task<TokenRespone> GenerateAndSaveTokensAsync(Users user, Token userToken)
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

            userToken.RefreshToken = BCrypt.Net.BCrypt.HashPassword(rawRefreshToken);
            userToken.RefreshTokenExpir = DateTime.UtcNow.AddDays(7);
            userToken.RefreshTokenRevokedAt = null;

            if (userToken.Id == 0)
            {
                await _genericRepositoryToken.AddAsync(userToken);
            }
            else
            {
                await _genericRepositoryToken.UpdateAsync(userToken);
            }

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