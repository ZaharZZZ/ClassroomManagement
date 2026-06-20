// Services/Interfaces/IAuthService.cs
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Models.ViewModels;

namespace ClassroomManagement.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Проверка логина и пароля, возврат пользователя при успехе
        /// </summary>
        Task<User?> ValidateUserAsync(string login, string password);

        /// <summary>
        /// Получение пользователя по ID
        /// </summary>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Обновление времени последнего входа
        /// </summary>
        Task UpdateLastLoginAsync(int userId);

        /// <summary>
        /// Хеширование пароля
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Проверка пароля
        /// </summary>
        bool VerifyPassword(string password, string hash);
    }
}