using Business.AdminAuth.Dtos;
using Business.AdminAuth.Helper;
using Business.AdminAuth.Interfaces;
using Business.Email.Validator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;



namespace Business.AdminAuth.Validator
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthService> _logger;
        private readonly EmailService _emailService;
        private readonly JWT _jwt;
        private readonly AppSettings _appSettings;
        private readonly SmtpSettings _smtpSettings;

        public AuthService(
            UserManager<AppUser> userManager,
            EmailService emailService,
            IOptions<JWT> jwt,
            ILogger<AuthService> logger,
            IOptions<AppSettings> appSettings,
            IOptions<SmtpSettings> smtpSettings)
        {
            _userManager = userManager;
            _emailService = emailService;
            _jwt = jwt.Value;
            _logger = logger;
            _appSettings = appSettings.Value;
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new RegistrationResult { Status = RegistrationStatus.UserAlreadyExists, Message = "User already exists." };
            }
            var passwordValidator = new PasswordValidator<AppUser>();
            var validationResult = await passwordValidator.ValidateAsync(_userManager, null, model.Password);
            if (!validationResult.Succeeded)
            {
                foreach (var error in validationResult.Errors)
                {
                    _logger.LogWarning($"Password validation error: {error.Description}");
                }
                return new RegistrationResult { Status = RegistrationStatus.PasswordValidationFailed, Message = "Password validation failed." };
            }
            var newUser = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
            };
            try
            {

                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to create role.");
                    return new RegistrationResult { Status = RegistrationStatus.OtherError, Message = "Failed to create user." };
                }
                return new RegistrationResult { Status = RegistrationStatus.Success, Message = "User registered successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during register.");
                return new RegistrationResult { Status = RegistrationStatus.OtherError, Message = "An error occurred during registration." };
            }
        }
        public async Task<string> LoginAsync(LoginDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    _logger.LogWarning("Invalid login attempt: User not found or invalid password.");
                    return null;
                }
                var token = await GenerateJwtToken(user);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return null;
            }
        }
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return true;
                }
                var temporaryPassword = GenerateTemporaryPassword();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, temporaryPassword);
                var message = new MailMessage(_smtpSettings.Username, email);
                message.Subject = "Temporary Password";
                message.Body = $"Dear Admin ,\n\n"
                             + $"Your temporary password is: {temporaryPassword}\n\n"
                             + $"Please log in using this temporary password and reset your password immediately.\n\n"
                             + $"Regards,\nEleve-Support Team";
                message.IsBodyHtml = false;
                var emailmodel = new EmailModel() { Body = message.Body, ToEmail = email, Subject = message.Subject, FromName = "Eleve-Support", ToName = "Admin" };
                _emailService.SendEmail(emailmodel);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing forgot password request.");
                return false;
            }
        }
        public async Task<ChangePasswordResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ChangePasswordResult { Success = false, ErrorMessage = "User not found." };
                }
                var passwordValidator = new PasswordValidator<AppUser>();
                var validationResult = await passwordValidator.ValidateAsync(_userManager, null, newPassword);
                if (!validationResult.Succeeded)
                {
                    var errorMessages = validationResult.Errors.Select(error => error.Description);
                    return new ChangePasswordResult { Success = false, ErrorMessage = string.Join(Environment.NewLine, errorMessages) };
                }
                var result = await _userManager.CheckPasswordAsync(user, currentPassword);
                if (!result)
                {
                    return new ChangePasswordResult { Success = false, ErrorMessage = "Current password is incorrect." };
                }
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (!changePasswordResult.Succeeded)
                {
                    return new ChangePasswordResult { Success = false, ErrorMessage = "Password change failed." };
                }

                return new ChangePasswordResult { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing user password.");
                return new ChangePasswordResult { Success = false, ErrorMessage = "An error occurred while changing user password." };
            }
        }
        #region private methods

        private async Task<string> GenerateJwtToken(AppUser user)
        {


            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            }
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredintials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredintials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        private string GenerateTemporaryPassword()
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()+[]{}|;:,.<>?";
            const int passwordLength = 8;
            var random = new Random();
            var passwordChars = new List<char>();
            passwordChars.Add(upperCase[random.Next(upperCase.Length)]);
            passwordChars.Add(lowerCase[random.Next(lowerCase.Length)]);
            passwordChars.Add(digits[random.Next(digits.Length)]);
            passwordChars.Add(specialChars[random.Next(specialChars.Length)]);
            string allChars = upperCase + lowerCase + digits + specialChars;
            for (int i = passwordChars.Count; i < passwordLength; i++)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }
            return new string(passwordChars.ToArray());
        }
        #endregion
    }
}

