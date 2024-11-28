using Business.AdminAuth.Dtos;
using Business.AdminAuth.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Business.AdminAuth.Interfaces
{
    public interface IAuthService
    {
        Task<RegistrationResult> RegisterAsync(RegisterDto model);
        Task<string> LoginAsync(LoginDto model);
        Task<ChangePasswordResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ForgotPasswordAsync(string email);
    }
}
