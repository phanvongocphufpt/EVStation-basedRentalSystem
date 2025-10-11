using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Repository.IRepositories;
using Service.EmailConfirmation;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public AuthService(IUserRepository userRepository, EmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public async Task<User> Authenticate(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            if (!user.IsEmailConfirmed)
                throw new Exception("Vui lòng xác nhận email trước khi đăng nhập.");

            return user;

        }

        public static string GenerateToken(int length = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] tokenData = new byte[length];
            rng.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }
        //public static string GenerateEmailToken(int length = 5)
        //{
        //    using var rng = RandomNumberGenerator.Create();
        //    byte[] tokenData = new byte[length];
        //    rng.GetBytes(tokenData);
        //    return Convert.ToBase64String(tokenData);
        //}
        public static string GenerateEmailToken()
        {
            Random random = new Random();
            string token = random.Next(100000, 999999).ToString();
            return token;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (string.IsNullOrWhiteSpace(user.Role))
                throw new ArgumentException("User role is required");


            var key = Encoding.ASCII.GetBytes(GenerateToken());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                     new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task Register(User user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Người dùng không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Mật khẩu không được để trống.", nameof(password));
            }

            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email đã được sử dụng.");
            }

            user.Role = string.IsNullOrWhiteSpace(user.Role) ? "Customer" : user.Role;

            user.PasswordHash = HashPassword(password);

            user.ConfirmEmailToken = GenerateEmailToken();
            user.IsEmailConfirmed = false;

            using (var transaction = await _userRepository.BeginTransactionAsync())
            {
                try
                {
                    await _emailService.SendConfirmationEmail(user.Email, user.ConfirmEmailToken);

                    await _userRepository.AddAsync(user);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    Console.WriteLine($"Lỗi khi đăng ký người dùng: {ex.Message}");

                    throw;
                }
            }
        }
    }
}
