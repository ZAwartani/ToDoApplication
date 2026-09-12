using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.BackgroundJob;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class AuthenticationServices : IAuthentications
    {
        private readonly UserManager<User> _UserRepo;
        private readonly IGenericRepository<Token, int> _TokenRepo;
        private readonly IGenericRepository<CodeOTP, int> _CodeRepo;
        private readonly ISpecificationBuilderFactory _Factory;
        private readonly IConfiguration _Configuration;

        public AuthenticationServices(
            UserManager<User> genericRepositoryUser,
            IGenericRepository<Token, int> genericRepositoryToken,
            IGenericRepository<CodeOTP, int> genericRepositoryCode,
            ISpecificationBuilderFactory factory,
            IConfiguration configuration)
        {
            _UserRepo = genericRepositoryUser;
            _TokenRepo = genericRepositoryToken;
            _CodeRepo = genericRepositoryCode;
            _Factory = factory;
            _Configuration = configuration;
        }

        public async Task<bool> ChangePasswordUserByAdminAsync(AdminRestPasswordUserDto adminRestPasswordUserDto)
        {
            var user = await _UserRepo.FindByEmailAsync(adminRestPasswordUserDto.Email);

            if (user == null)
                return false;

            var token = await _UserRepo.GeneratePasswordResetTokenAsync(user);

            var result = await _UserRepo.ResetPasswordAsync(
                user,
                token,
                adminRestPasswordUserDto.Password);

            return result.Succeeded;
        }

        public async Task<IReadOnlyList<UserListDto>?> ListUserAsync()
        {
            var users = _UserRepo.Users
                .Select(x => new UserListDto(x.Id, x.Email!))
                .ToList();

            return await Task.FromResult(users);
        }

        public async Task<bool> CreateAdminUserAsync(RegisterNewUserDto newUser)
        {
            var existingUser = await _UserRepo.FindByEmailAsync(newUser.Email);

            if (existingUser != null)
                return false;

            var user = new User(
                newUser.UserName,
                newUser.Email);

            var result = await _UserRepo.CreateAsync(
                user,
                newUser.Password);

            if (!result.Succeeded)
                return false;

            var roleResult = await _UserRepo.AddToRoleAsync(
                user,
                "Admin");

            return roleResult.Succeeded;
        }

        public async Task<bool> RemoveUserAsync(string email)
        {
            var user = await _UserRepo.FindByEmailAsync(email);

            if (user == null)
                return false;

            var result = await _UserRepo.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task RegisterNewUserAsync(RegisterNewUserDto registerDto)
        {
            var user = new User(
                registerDto.UserName,
                registerDto.Email);

            var result = await _UserRepo.CreateAsync(
                user,
                registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"User registration failed: {errors}");
            }

            var roleResult = await _UserRepo.AddToRoleAsync(
                user,
                "User");

            if (!roleResult.Succeeded)
            {
                var roleErrors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to assign role: {roleErrors}");
            }
        }

        public async Task<TokenResponeDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _UserRepo.FindByEmailAsync(loginDto.Email);

            if (user == null)
                throw new Exception("Invalid email or password");

            var isPasswordValid = await _UserRepo.CheckPasswordAsync(
                user,
                loginDto.Password);

            if (!isPasswordValid)
                throw new Exception("Invalid email or password");

            var tokenSpec = _Factory.CreateBuilder<Token>()
                .AddCriteria(t => t.UserId == user.Id)
                .AddCriteria(t => !t.RefreshTokenRevokedAt.HasValue)
                .ApplyOrderBy(t => t.CreatedDate, true);

            var existingToken = await _TokenRepo.GetAsync(tokenSpec);

            if (existingToken != null)
            {
                existingToken.RefreshTokenRevokedAt = DateTime.UtcNow;

                await _TokenRepo.UpdateAsync(existingToken);
            }

            return await GenerateAndSaveTokensAsync(user);
        }

        public async Task LogoutAsync(LogoutDto logoutDto)  
        {
            var user = await _UserRepo.FindByEmailAsync(logoutDto.Email);

            if (user == null)
                throw new Exception("User not found");

            var tokenSpec = _Factory.CreateBuilder<Token>()
                .AddCriteria(t => t.UserId == user.Id)
                .AddCriteria(t => t.RefreshToken == logoutDto.RefreshToken)
                .AddCriteria(t => !t.RefreshTokenRevokedAt.HasValue);

            var userToken = await _TokenRepo.GetAsync(tokenSpec);

            if (userToken == null)
                throw new Exception(
                    "The refresh token is invalid or already revoked");

            userToken.RefreshTokenRevokedAt = DateTime.UtcNow;

            await _TokenRepo.UpdateAsync(userToken);
        }

        public async Task<string?> ChangePasswordAsync(ForgetPasswordOTPDto forgetDto)
        {
            var user = await _UserRepo.FindByEmailAsync(forgetDto.Email);

            if (user == null)
                throw new Exception("User not found");

            var otpSpec = _Factory.CreateBuilder<CodeOTP>()
                .AddCriteria(c => c.UserId == user.Id)
                .AddCriteria(c => c.Code == forgetDto.OTP);

            var otp = await _CodeRepo.GetAsync(otpSpec);

            if (otp == null)
                throw new Exception("Invalid verification code");

            if (otp.CodeExpirDate < DateTime.UtcNow)
                throw new Exception("Verification code has expired");

            var result = await ResetPasswordManualAsync(
                user,
                forgetDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new Exception(
                    $"Password reset failed: {errors}");
            }

            await _CodeRepo.DeleteAsync(otp.Id);

            return "Password Changed Successfully";
        }

        

        public async Task<TokenResponeDto?> RefreshTokenAsync(
            RefreshTokenDto refreshToken)
        {
            var tokenSpec = _Factory.CreateBuilder<Token>()
                .AddCriteria(t => t.User.Email == refreshToken.Email)
                .AddCriteria(t => t.RefreshToken == refreshToken.Refreshtoken)
                .AddInclude(t => t.User)
                .ApplyOrderBy(t => t.CreatedDate, true);

            var userToken = await _TokenRepo.GetAsync(tokenSpec);

            if (userToken == null)
                throw new Exception("Invalid refresh token");

            var user = userToken.User;

            if (userToken.RefreshTokenRevokedAt != null)
                throw new Exception("Refresh token has been revoked");

            if (userToken.RefreshTokenExpir < DateTime.UtcNow)
                throw new Exception("Refresh token has expired");

            userToken.RefreshTokenRevokedAt = DateTime.UtcNow;

            await _TokenRepo.UpdateAsync(userToken);

            return await GenerateAndSaveTokensAsync(user);
        }

        private async Task<TokenResponeDto> GenerateAndSaveTokensAsync(User user)
        {
            var utcNow = DateTime.UtcNow;

            var roles = await _UserRepo.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    user.Email ?? string.Empty),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role));
            }

            var jwtKey = _Configuration["JWT:Key"]
                ?? throw new InvalidOperationException(
                    "JWT key is not configured");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _Configuration["JWT:Issuer"],
                audience: _Configuration["JWT:Audience"],
                claims: claims,
                expires: utcNow.AddMinutes(15),
                signingCredentials: credentials);

            var accessToken =
                new JwtSecurityTokenHandler().WriteToken(jwt);

            var rawRefreshToken = GenerateRefreshToken();

            var userToken = new Token(
                user.Id,
                rawRefreshToken,
                utcNow.AddDays(7),
                null,
                utcNow);

            await _TokenRepo.AddAsync(userToken);

            return new TokenResponeDto(
                accessToken,
                rawRefreshToken);
        }

        private string GenerateRefreshToken()
        {
            var bytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        private async Task<IdentityResult> ResetPasswordManualAsync(
            User user,
            string newPassword)
        {
            var resetToken =
                await _UserRepo.GeneratePasswordResetTokenAsync(user);

            return await _UserRepo.ResetPasswordAsync(
                user,
                resetToken,
                newPassword);
        }
    }
}