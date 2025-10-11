using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        Task<User> Authenticate(string email, string password);
        Task Register(User user, string password);
        Task ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string otp, string newPassword);
    }
}
