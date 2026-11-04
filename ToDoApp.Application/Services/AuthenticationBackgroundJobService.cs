using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces.BackgroundJob;
using ToDoApp.Application.Interfaces.Factory;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Domain.Entity;

namespace ToDoApp.Application.Services
{
    public class AuthenticationBackgroundJobService : IAuthenticationBackgroundJob
    {
        private readonly UserManager<User> _UserRepo;
        private readonly IGenericRepository<CodeOTP, int> _CodeRepo;
        private readonly ISpecificationBuilderFactory _Factory;

        public AuthenticationBackgroundJobService(UserManager<User> UserRepo , IGenericRepository<CodeOTP, int> CodeRepo, ISpecificationBuilderFactory Factory)
        {
            _UserRepo = UserRepo;
            _CodeRepo = CodeRepo;
            _Factory = Factory;
        }
        public async Task SendOTPAsync(SendOTPDto sendOTPDto)
        {
            var user = await _UserRepo.FindByEmailAsync(sendOTPDto.Email);

            if (user == null)
                throw new Exception("User not found");

            string codeOTPToEmail =
                RandomNumberGenerator.GetInt32(10000, 99999).ToString();

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    "TodoApp",
                    "zakaryawork005@gmail.com"));

            message.To.Add(new MailboxAddress(
                    "User",
                    sendOTPDto.Email));

            message.Subject =
                "Reset Your Password - TodoApp";

            message.Body = new TextPart("plain")
            {
                Text =
                    $"This is your verification code: {codeOTPToEmail}. It will expire in 5 minutes."
            };

            await _CodeRepo.RemoveRangeAsync(
                await _CodeRepo.ListAsync(
                    _Factory
                        .CreateBuilder<CodeOTP>()
                        .AddCriteria(c => c.UserId == user.Id)));

            var newOtp = new CodeOTP(
                codeOTPToEmail,
                DateTime.UtcNow.AddMinutes(5),
                user.Id);

            await _CodeRepo.AddAsync(newOtp);

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(
                    "smtp.gmail.com",
                    587,
                    MailKit.Security.SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    "zakaryawork005@gmail.com",
                    "qkukqdjeaqbxkjod");

                await client.SendAsync(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error sending email: {ex.Message}");
                throw;

            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
