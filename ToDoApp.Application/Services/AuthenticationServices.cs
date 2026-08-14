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
        private readonly IGenericRepository<User, int> _UserRepo;
        private readonly IGenericRepository<Token, int> _TokenRepo;
        private readonly IGenericRepository<CodeOTP, int> _CodeRepo;
        private readonly IGenericRepository<ToDoItem, int> _TodoRepo;
        private readonly IGenericRepository<Category, int> _CategoryRepo;

        private readonly ISpecificationBuilderFactory _Factory;
        private readonly IConfiguration _Configuration;

        public AuthenticationServices(
            IGenericRepository<User, int> genericRepositoryUser,
            IGenericRepository<Token, int> genericRepositoryToken,
            IGenericRepository<CodeOTP, int> genericRepositoryCode,
            IGenericRepository<ToDoItem, int> genericRepositoryTodo,
            IGenericRepository<Category, int> genericRepositoryCategory,
            ISpecificationBuilderFactory factory,
            IConfiguration configuration)
        {
            _UserRepo = genericRepositoryUser;
            _TokenRepo = genericRepositoryToken;
            _CodeRepo = genericRepositoryCode;
            _Factory = factory;
            _TodoRepo = genericRepositoryTodo;
            _CategoryRepo = genericRepositoryCategory;
            _Configuration = configuration;
        }
        public async Task<bool> ChangePasswordUserByAdmin(
           AdminRestPasswordUserDto AdminRestPasswordUserDto)
        {
            var Spec = _Factory.CreateBuilder<User>()
                .AddCriteria(x => x.Email == AdminRestPasswordUserDto.Email);


            var User = await _UserRepo.GetAsync(Spec);

            if (User == null)
                return false;


            var PasswordHash = BCrypt.Net.BCrypt
                .HashPassword(AdminRestPasswordUserDto.Password);


            User.UpdatePasswordByAdmin(PasswordHash);


            await _UserRepo.UpdateAsync(User);

            return true;
        }

        public async Task<IReadOnlyList<UserListDTO>> ListOfUserForAdmin()
        {
            var Users = await _UserRepo.GetAllAsync();


            return Users.Select(x => new UserListDTO(
                    x.Id,
                    x.Email
                ))
                .ToList();
        }


        public async Task<bool> CreateAdminUser(
    RegisterNewUserDto NewUser)
        {

            var EmailSpec = _Factory.CreateBuilder<User>()
                .AddCriteria(x => x.Email == NewUser.Email);

            var ExistingUser =
                (await _UserRepo.GetAsync(EmailSpec));

            if (ExistingUser != null)
                return false;

            var PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(NewUser.Password);

            var user = new User(
                NewUser.UserName,
                NewUser.Email,
                PasswordHash,
                enumUserRole.UserAdmin
            );

            await _UserRepo.AddAsync(user);

            return true;
        }

        public async Task<bool> RemoveUserAsync(string Email)
        {
            var Spec = _Factory.CreateBuilder<User>()
                .AddCriteria(x => x.Email == Email);

            var User =
                (await _UserRepo.GetAsync(Spec));

            if (User == null)
                return false;

            await _UserRepo.DeleteAsync(User.Id);

            return true;
        }
        public async Task RegisterNewUserAsync(RegisterNewUserDto RegisterDto)
        {
            var PasswordHash = BCrypt.Net.BCrypt.HashPassword(RegisterDto.Password);
            var User = new User(
                RegisterDto.UserName,
                RegisterDto.Email,
                PasswordHash,
                enumUserRole.User
            );

            await _UserRepo.AddAsync(User);
        }

        public async Task<TokenRespone?> Login(LoginDto LoginDto)
        {
            var Spec = _Factory.CreateBuilder<User>()
                .AddCriteria(u => u.Email == LoginDto.Email);

            var User = await _UserRepo.GetAsync(Spec);

            if (User == null || !BCrypt.Net.BCrypt.Verify(LoginDto.Password, User.Password))
                throw new Exception("Invalid email or password");


            var TokenSpec = _Factory.CreateBuilder<Token>()
                 .AddCriteria(t => t.UserId == User.Id)
                 .ApplyOrderBy(t => t.CreatedDate, true);

            var UserToken = await _TokenRepo.GetAsync(TokenSpec);
            if (UserToken.RefreshTokenRevokedAt != null)
            {
                UserToken.RefreshTokenRevokedAt = DateTime.UtcNow;
                await _TokenRepo.UpdateAsync(UserToken);
            }

            var TokenResponse = await GenerateAndSaveTokensAsync(User);
            return TokenResponse;
        }

        public async Task Logout(LogoutDto LogoutDto)
        {
            var Spec = _Factory.CreateBuilder<User>()
                .AddCriteria(u => u.Email == LogoutDto.Email);

            var User = (await _UserRepo.GetAsync(Spec));

            if (User == null)
                throw new Exception("User not found");

            var TokenSpec = _Factory.CreateBuilder<Token>()
                .AddCriteria(t => t.UserId == User.Id)
                .ApplyOrderBy(t => t.CreatedDate, true);

            var UserToken = await _TokenRepo.GetAsync(TokenSpec);

            if (!BCrypt.Net.BCrypt.Verify(LogoutDto.RefreshToken, UserToken.RefreshToken))
                throw new Exception("The refresh token is invalid");

            UserToken.RefreshTokenRevokedAt = DateTime.UtcNow;

            await _TokenRepo.UpdateAsync(UserToken);
        }

        public async Task<string?> ChangePassword(ForgetPasswordOTP ForgetDto)
        {
            var UserSpec = _Factory.CreateBuilder<User>()
                .AddCriteria(u => u.Email == ForgetDto.Email);

            var User = await _UserRepo.GetAsync(UserSpec);

            if (User == null)
                throw new Exception("User not found");

            var otpSpec = _Factory.CreateBuilder<CodeOTP>()
                .AddCriteria(c => c.UserId == User.Id);

            var Otp = await _CodeRepo.GetAsync(otpSpec);

            if (Otp == null)
                throw new Exception("Invalid verification code");

            if (Otp.Code != ForgetDto.OTP)
                throw new Exception("Invalid verification code");

            if (Otp.CodeExpirDate< DateTime.UtcNow)
                throw new Exception("Verification code has expired");

            User.Password = BCrypt.Net.BCrypt.HashPassword(ForgetDto.Password);
            await _UserRepo.UpdateAsync(User);

            await _CodeRepo.DeleteAsync(Otp.Id);

            return "Password Changed Successfully";
        }

        public async Task<bool> SendCodeAsync(SendOTP sendOTPDto)
        {
            var Spec = _Factory.CreateBuilder<User>()
                .AddCriteria(u => u.Email == sendOTPDto.Email);

            var user = (await _UserRepo.GetAsync(Spec));

            if (user == null)
                throw new Exception("User not found");

            string CodeOTPToEmail = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
            MimeMessage Message = new MimeMessage();
            Message.From.Add(new MailboxAddress("TodoApp", "zakaryawork005@gmail.com"));
            Message.To.Add(new MailboxAddress("User", sendOTPDto.Email));
            Message.Subject = "Reset Your Password - TodoApp";

            Message.Body = new TextPart("plain")
            {
                Text = $"This is your verification code: {CodeOTPToEmail}. It will expire in 5 minutes."
            };

            await _CodeRepo.RemoveRangeAsync(await _CodeRepo.ListAsync(_Factory.CreateBuilder<CodeOTP>().AddCriteria(c => c.UserId == user.Id)));
            
            var NewOtp = new CodeOTP
            (
                    CodeOTPToEmail,
                    DateTime.UtcNow.AddMinutes(5),
                    user.Id
            );

            await _CodeRepo.AddAsync(NewOtp);

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("zakaryawork005@gmail.com", "qkukqdjeaqbxkjod");
                    await client.SendAsync(Message);
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
            var tokenSpec = _Factory.CreateBuilder<Token>()
                .AddCriteria(t => t.User.Email == refreshToken.Email)
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

            if (!BCrypt.Net.BCrypt.Verify(
                    refreshToken.Refreshtoken,
                    userToken.RefreshToken))
                throw new Exception("Invalid refresh token");


            userToken.RefreshTokenRevokedAt = DateTime.UtcNow;

            await _TokenRepo.UpdateAsync(userToken);

            return await GenerateAndSaveTokensAsync(user);
        }

        private async Task<TokenRespone> GenerateAndSaveTokensAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtKey = _Configuration["JWT:Key"]
                ?? throw new Exception("JWT key is not configured");

            var jwtIssuer = _Configuration["JWT:Issuer"];
            var jwtAudience = _Configuration["JWT:Audience"];

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.RoleId.ToString())
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            var accessToken = tokenHandler.WriteToken(jwt);

            var rawRefreshToken = GenerateRefreshToken();

            var hashedRefreshToken = BCrypt.Net.BCrypt.HashPassword(rawRefreshToken);

            var userToken = new Token
            (
                user.Id,
                hashedRefreshToken,
                DateTime.UtcNow.AddDays(7),
                null,
                DateTime.UtcNow
            );

            await _TokenRepo.AddAsync(userToken);

            return new TokenRespone(
                accessToken,
                rawRefreshToken);
        }

        private string GenerateRefreshToken()
        {
            var Bytes = new byte[64];
            using var RNG = RandomNumberGenerator.Create();
            RNG.GetBytes(Bytes);
            return Convert.ToBase64String(Bytes);
        }
    }
}