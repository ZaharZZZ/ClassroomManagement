// Controllers/AccountController.cs — полная версия
using ClassroomManagement.Data;
using ClassroomManagement.Helpers;
using ClassroomManagement.Models.ViewModels;
using ClassroomManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAuditService _auditService;
        private readonly ApplicationDbContext _context;

        public AccountController(
            IAuthService authService,
            IAuditService auditService,
            ApplicationDbContext context)
        {
            _authService = authService;
            _auditService = auditService;
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Проверяем существование пользователя
            var userExists = await _context.Users
                .AnyAsync(u => u.Login == model.Login);

            if (!userExists)
            {
                ModelState.AddModelError("",
                    $"Пользователь '{model.Login}' не найден. " +
                    "Перейдите по адресу /Temp/CreateUsers для создания тестовых пользователей.");
                return View(model);
            }

            var user = await _authService.ValidateUserAsync(model.Login, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Неверный логин или пароль");
                return View(model);
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError("",
                    "Учётная запись заблокирована. Обратитесь к администратору.");
                return View(model);
            }

            // Записываем в сессию
            HttpContext.Session.SetInt32(SessionHelper.UserId, user.Id);
            HttpContext.Session.SetString(SessionHelper.UserLogin, user.Login);
            HttpContext.Session.SetString(SessionHelper.UserRole, user.Role.Name);
            HttpContext.Session.SetString(SessionHelper.UserFullName,
                user.Teacher?.FullName ?? user.Login);

            if (user.TeacherId.HasValue)
                HttpContext.Session.SetInt32(SessionHelper.TeacherId, user.TeacherId.Value);

            await _authService.UpdateLastLoginAsync(user.Id);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _auditService.LogAsync(user.Id, "Login",
                $"Пользователь {user.Login} вошёл в систему", ip);

            return user.Role.Name switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Dispatcher" => RedirectToAction("Index", "Dispatcher"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session);
            var login = HttpContext.Session.GetString(SessionHelper.UserLogin);

            if (userId.HasValue)
            {
                await _auditService.LogAsync(userId.Value, "Logout",
                    $"Пользователь {login} вышел из системы");
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied() => View();
    }
}