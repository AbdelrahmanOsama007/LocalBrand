using Business.AdminAuth.Dtos;
using Business.AdminAuth.Interfaces;
using Business.Email.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Enums;
using System.Security.Claims;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var registrationResult = await _authService.RegisterAsync(model);

                return registrationResult.Status switch
                {
                    RegistrationStatus.Success => Ok(new { message = "User registered successfully." }),
                    RegistrationStatus.UserAlreadyExists => Conflict(new { error = "User already exists." }),
                    RegistrationStatus.PasswordValidationFailed => BadRequest(new { error = "Password validation failed." }),
                    RegistrationStatus.OtherError => StatusCode(500, new { error = "An error occurred during registration." }),
                    _ => StatusCode(500, new { error = "An unexpected error occurred." })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during registration.");
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var token = await _authService.LoginAsync(model);
                if (token == null)
                {
                    _logger.LogWarning("Invalid login attempt: Invalid token.");
                    return Unauthorized(new { error = "Invalid email or password." });
                }

                _logger.LogInformation("User logged in successfully.");
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during login.");
                return StatusCode(500, new { error = "An unexpected error occurred during login." });
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDto emaildto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var isSuccessful = await _authService.ForgotPasswordAsync(emaildto.Email);
                    if (isSuccessful)
                    {
                        _logger.LogInformation($"Temporary password sent successfully to {emaildto.Email}.");
                        return Ok(new { message = "a message sent successfully. Please check your email." });
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to send temporary password to {emaildto.Email}.");
                        return BadRequest(new { error = "Failed to process forgot password request." });
                    }
                }
                else
                {
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    return BadRequest(new { Errors = errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during forgot password request.");
                return StatusCode(500, new { error = "An unexpected error occurred during forgot password request." });
            }
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest("Invalid token.");
                }

                var result = await _authService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
                if (result.Success)
                {
                    _logger.LogInformation($"Password changed successfully for user with ID '{userId}'.");
                    return Ok(new { message = "Password changed successfully." });
                }
                else
                {
                    _logger.LogWarning($"Failed to change password for user with ID '{userId}'. Error: {result.ErrorMessage}");
                    return BadRequest(new { error = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during password change.");
                return StatusCode(500, new { error = "An unexpected error occurred during password change." });
            }
        }
    }
}
