using ClassroomManagement.Data;
using ClassroomManagement.Helpers;
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Models.ViewModels;
using ClassroomManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly IClassroomService _classroomService;
        private readonly IAuditService _auditService;
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AdminController(
            IClassroomService classroomService,
            IAuditService auditService,
            IAuthService authService,
            ApplicationDbContext context)
        {
            _classroomService = classroomService;
            _auditService = auditService;
            _authService = authService;
            _context = context;
        }

        private IActionResult? CheckAccess()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
                return RedirectToAction("Login", "Account");
            if (!SessionHelper.IsAdmin(HttpContext.Session))
                return RedirectToAction("AccessDenied", "Account");
            return null;
        }

        // GET: /Admin/Index
        public IActionResult Index()
        {
            var access = CheckAccess();
            if (access != null) return access;
            return View();
        }

        // ============================================
        // УПРАВЛЕНИЕ КАБИНЕТАМИ
        // ============================================

        public async Task<IActionResult> Classrooms()
        {
            var access = CheckAccess();
            if (access != null) return access;
            var classrooms = await _classroomService.GetAllClassroomsAsync();
            return View(classrooms);
        }

        public async Task<IActionResult> CreateClassroom()
        {
            var access = CheckAccess();
            if (access != null) return access;
            var model = await _classroomService.GetClassroomFormViewModelAsync();
            return View("ClassroomForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClassroom(ClassroomFormViewModel model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (!ModelState.IsValid)
            {
                var refreshed = await _classroomService.GetClassroomFormViewModelAsync();
                model.Buildings = refreshed.Buildings;
                model.RoomTypes = refreshed.RoomTypes;
                model.Conditions = refreshed.Conditions;
                model.Departments = refreshed.Departments;
                model.EquipmentList = refreshed.EquipmentList;
                return View("ClassroomForm", model);
            }

            var result = await _classroomService.CreateClassroomAsync(model);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Classrooms");
        }

        public async Task<IActionResult> EditClassroom(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;
            var model = await _classroomService.GetClassroomFormViewModelAsync(id);
            return View("ClassroomForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClassroom(ClassroomFormViewModel model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (!ModelState.IsValid)
            {
                var refreshed = await _classroomService.GetClassroomFormViewModelAsync(model.Id);
                model.Buildings = refreshed.Buildings;
                model.RoomTypes = refreshed.RoomTypes;
                model.Conditions = refreshed.Conditions;
                model.Departments = refreshed.Departments;
                model.EquipmentList = refreshed.EquipmentList;
                return View("ClassroomForm", model);
            }

            var result = await _classroomService.UpdateClassroomAsync(model);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Classrooms");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClassroom(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;
            var result = await _classroomService.DeleteClassroomAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Classrooms");
        }

        // ============================================
        // УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ
        // ============================================

        public async Task<IActionResult> Users()
        {
            var access = CheckAccess();
            if (access != null) return access;

            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Teacher)
                .OrderBy(u => u.Role.Name)
                .ThenBy(u => u.Login)
                .ToListAsync();

            // Передаём данные для модального окна создания
            ViewBag.Roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
            ViewBag.Teachers = await _context.Teachers
                .OrderBy(t => t.FullName).ToListAsync();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(
            string login, string password, int roleId, int? teacherId)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Логин и пароль обязательны";
                return RedirectToAction("Users");
            }

            if (await _context.Users.AnyAsync(u => u.Login == login))
            {
                TempData["Error"] = $"Пользователь с логином '{login}' уже существует";
                return RedirectToAction("Users");
            }

            var user = new User
            {
                Login = login,
                PasswordHash = _authService.HashPassword(password),
                RoleId = roleId,
                TeacherId = teacherId,
                IsBlocked = false,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var adminId = SessionHelper.GetUserId(HttpContext.Session);
            await _auditService.LogAsync(adminId, "CreateUser",
                $"Создан пользователь {login}",
                HttpContext.Connection.RemoteIpAddress?.ToString());

            TempData["Success"] = $"Пользователь '{login}' успешно создан";
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserBlock(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Пользователь не найден";
                return RedirectToAction("Users");
            }

            user.IsBlocked = !user.IsBlocked;
            await _context.SaveChangesAsync();

            var adminId = SessionHelper.GetUserId(HttpContext.Session);
            await _auditService.LogAsync(adminId,
                user.IsBlocked ? "BlockUser" : "UnblockUser",
                $"Пользователь {user.Login}",
                HttpContext.Connection.RemoteIpAddress?.ToString());

            TempData["Success"] = user.IsBlocked
                ? $"Пользователь '{user.Login}' заблокирован"
                : $"Пользователь '{user.Login}' разблокирован";

            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int userId, string newPassword)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["Error"] = "Пароль должен содержать минимум 6 символов";
                return RedirectToAction("Users");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Пользователь не найден";
                return RedirectToAction("Users");
            }

            user.PasswordHash = _authService.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            var adminId = SessionHelper.GetUserId(HttpContext.Session);
            await _auditService.LogAsync(adminId, "ChangePassword",
                $"Смена пароля для пользователя {user.Login}",
                HttpContext.Connection.RemoteIpAddress?.ToString());

            TempData["Success"] = $"Пароль пользователя '{user.Login}' изменён";
            return RedirectToAction("Users");
        }

        // ============================================
        // УПРАВЛЕНИЕ ОТДЕЛЕНИЯМИ
        // ============================================

        public async Task<IActionResult> Departments()
        {
            var access = CheckAccess();
            if (access != null) return access;

            var departments = await _context.Departments
                .Include(d => d.Head)
                .Include(d => d.Teachers)
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.Teachers = await _context.Teachers
                .OrderBy(t => t.FullName).ToListAsync();

            return View(departments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment(Department model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Abbreviation))
            {
                TempData["Error"] = "Наименование и аббревиатура обязательны";
                return RedirectToAction("Departments");
            }

            _context.Departments.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Отделение '{model.Name}' добавлено";
            return RedirectToAction("Departments");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(Department model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            var dept = await _context.Departments.FindAsync(model.Id);
            if (dept == null)
            {
                TempData["Error"] = "Отделение не найдено";
                return RedirectToAction("Departments");
            }

            dept.Name = model.Name;
            dept.Abbreviation = model.Abbreviation;
            dept.HeadId = model.HeadId;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Отделение '{dept.Name}' обновлено";
            return RedirectToAction("Departments");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;

            var dept = await _context.Departments.FindAsync(id);
            if (dept == null)
            {
                TempData["Error"] = "Отделение не найдено";
                return RedirectToAction("Departments");
            }

            // Проверяем есть ли преподаватели
            var hasTeachers = await _context.Teachers
                .AnyAsync(t => t.DepartmentId == id);
            if (hasTeachers)
            {
                TempData["Error"] =
                    "Нельзя удалить отделение: есть привязанные преподаватели";
                return RedirectToAction("Departments");
            }

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Отделение '{dept.Name}' удалено";
            return RedirectToAction("Departments");
        }

        // ============================================
        // ЖУРНАЛ АУДИТА
        // ============================================

        public async Task<IActionResult> AuditLog(int page = 1)
        {
            var access = CheckAccess();
            if (access != null) return access;

            var logs = await _auditService.GetLogsAsync(page, 50);
            ViewBag.CurrentPage = page;
            return View(logs);
        }
    }
}