// Services/AuthService.cs
using ClassroomManagement.Data;
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> ValidateUserAsync(string login, string password)
        {
            _logger.LogInformation("Попытка входа: логин={Login}", login);

            // Ищем пользователя по логину
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
            {
                _logger.LogWarning("Пользователь не найден: {Login}", login);
                return null;
            }

            _logger.LogInformation("Пользователь найден: {Login}, заблокирован: {Blocked}",
                login, user.IsBlocked);

            // Проверяем пароль
            bool passwordValid = false;
            try
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка проверки пароля для {Login}: {Error}",
                    login, ex.Message);
                return null;
            }

            if (!passwordValid)
            {
                _logger.LogWarning("Неверный пароль для пользователя: {Login}", login);
                return null;
            }

            if (user.IsBlocked)
            {
                _logger.LogWarning("Попытка входа заблокированного пользователя: {Login}", login);
                return null;
            }

            _logger.LogInformation("Успешный вход: {Login}, роль: {Role}",
                login, user.Role.Name);

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}