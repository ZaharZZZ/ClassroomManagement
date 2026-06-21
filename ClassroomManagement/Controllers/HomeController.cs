using ClassroomManagement.Data;
using ClassroomManagement.Helpers;
using ClassroomManagement.Models.ViewModels;
using ClassroomManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly IBookingService _bookingService;
        private readonly ApplicationDbContext _context;

        public HomeController(
            IScheduleService scheduleService,
            IBookingService bookingService,
            ApplicationDbContext context)
        {
            _scheduleService = scheduleService;
            _bookingService = bookingService;
            _context = context;
        }

        // -----------------------------------------------
        // GET: /Home/Index
        // -----------------------------------------------
        public async Task<IActionResult> Index(
            DateTime? date = null,
            int pair = 1,
            int? buildingId = null,
            int? floor = null,
            int? departmentId = null)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
                return RedirectToAction("Login", "Account");

            var selectedDate = date ?? DateTime.Today;
            var selectedPair = (pair >= 1 && pair <= 5) ? pair : 1;

            var viewModel = await _scheduleService.GetScheduleViewModelAsync(
                selectedDate,
                selectedPair,
                buildingId,
                floor,
                departmentId);

            return View(viewModel);
        }

        // -----------------------------------------------
        // GET: /Home/CellInfo  (AJAX)
        // -----------------------------------------------
        [HttpGet]
        public async Task<IActionResult> CellInfo(
            int classroomId, string date, int pair)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
                return Unauthorized();

            if (!DateOnly.TryParse(date, out var dateOnly))
                return BadRequest();

            var info = await _scheduleService.GetCellInfoAsync(classroomId, dateOnly, pair);
            return Json(info);
        }

        // -----------------------------------------------
        // GET: /Home/BookingForm  (AJAX — для преподавателя)
        // -----------------------------------------------
        [HttpGet]
        public async Task<IActionResult> BookingForm(
            int classroomId, string date, int pair)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
                return Content("<div class='alert alert-danger'>Необходима авторизация</div>");

            if (!SessionHelper.IsTeacher(HttpContext.Session))
                return Content("<div class='alert alert-danger'>Доступ только для преподавателей</div>");

            if (!DateOnly.TryParse(date, out var dateOnly))
                return Content("<div class='alert alert-danger'>Неверный формат даты</div>");
            if (dateOnly.DayOfWeek == DayOfWeek.Sunday)
                return Content("<div class='alert alert-danger'>Резервирование на воскресенье недоступно</div>");

            var pairs = await _scheduleService.GetPairsAsync();
            var selectedPair = pairs.FirstOrDefault(p => p.Number == pair);
            if (selectedPair == null)
                return Content("<div class='alert alert-danger'>Неверный номер пары</div>");

            // Загружаем кабинет с оснащением
            var classroom = await _context.Classrooms
                .Include(c => c.RoomType)
                .Include(c => c.ClassroomEquipments)
                    .ThenInclude(ce => ce.Equipment)
                .FirstOrDefaultAsync(c => c.Id == classroomId);

            var model = new CreateBookingViewModel
            {
                ClassroomId = classroomId,
                ClassroomNumber = classroom?.Number ?? classroomId.ToString(),
                PairNumber = pair,
                PairTime = selectedPair.DisplayName,
                EventDateStr = dateOnly.ToString("yyyy-MM-dd"),
                StartTimeStr = selectedPair.StartTime,
                EndTimeStr = selectedPair.EndTime,
                IsUrgent = dateOnly == DateOnly.FromDateTime(DateTime.Today),
                ClassroomCapacity = classroom?.Capacity ?? 0,
                RoomTypeName = classroom?.RoomType?.Name ?? "",
                EquipmentNote = classroom?.EquipmentNote,
                ClassroomEquipments = classroom?.ClassroomEquipments
                    .Select(ce => ce.Equipment.Name).ToList()
                    ?? new List<string>()
            };

            return PartialView("_BookingForm", model);
        }

        // -----------------------------------------------
        // POST: /Home/SubmitBooking  (AJAX)
        // -----------------------------------------------
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SubmitBooking(
            int classroomId,
            string eventDateStr,
            string startTimeStr,
            string endTimeStr,
            int pairNumber,
            string purpose,
            string periodicity = "once",
            string? periodEndDateStr = null)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
                return Json(new { success = false, message = "Необходима авторизация" });

            if (!SessionHelper.IsTeacher(HttpContext.Session))
                return Json(new { success = false, message = "Доступ только для преподавателей" });

            var teacherId = SessionHelper.GetTeacherId(HttpContext.Session);
            if (!teacherId.HasValue)
                return Json(new { success = false, message = "Ошибка идентификации преподавателя" });

            if (!DateOnly.TryParse(eventDateStr, out var eventDate))
                return Json(new { success = false, message = "Неверный формат даты" });
            if (eventDate.DayOfWeek == DayOfWeek.Sunday)
                return Json(new { success = false, message = "Резервирование на воскресенье запрещено" });

            if (!TimeOnly.TryParse(startTimeStr, out var startTime))
                return Json(new { success = false, message = "Неверный формат времени начала" });

            if (!TimeOnly.TryParse(endTimeStr, out var endTime))
                return Json(new { success = false, message = "Неверный формат времени окончания" });

            if (string.IsNullOrWhiteSpace(purpose))
                return Json(new { success = false, message = "Укажите цель резервирования" });

            DateOnly? periodEndDate = null;
            if (!string.IsNullOrEmpty(periodEndDateStr) &&
                DateOnly.TryParse(periodEndDateStr, out var ped))
                periodEndDate = ped;

            // Проверяем коллизии
            var hasConflict = await _bookingService.HasConflictAsync(
                classroomId, eventDate, startTime, endTime);
            if (hasConflict)
                return Json(new { success = false, message = "Кабинет уже занят на это время" });

            var pendingStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.Name == "Pending");
            if (pendingStatus == null)
                return Json(new { success = false, message = "Ошибка конфигурации системы" });

            var isUrgent = eventDate == DateOnly.FromDateTime(DateTime.Today);

            var booking = new ClassroomManagement.Models.Entities.Booking
            {
                TeacherId = teacherId.Value,
                ClassroomId = classroomId,
                EventDate = eventDate,
                StartTime = startTime,
                EndTime = endTime,
                PairNumber = pairNumber,
                Purpose = purpose.Trim(),
                Periodicity = periodicity,
                PeriodEndDate = periodEndDate,
                StatusId = pendingStatus.Id,
                IsUrgent = isUrgent,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var classroom = await _context.Classrooms.FindAsync(classroomId);

            return Json(new
            {
                success = true,
                message = $"Заявка на кабинет {classroom?.Number} успешно подана! Ожидайте подтверждения от диспетчера."
            });
        }

        // GET: /Home/TestSession — временный метод диагностики
        public IActionResult TestSession()
        {
            var userId = HttpContext.Session.GetInt32(SessionHelper.UserId);
            var role = HttpContext.Session.GetString(SessionHelper.UserRole);
            var teachId = HttpContext.Session.GetInt32(SessionHelper.TeacherId);

            return Json(new
            {
                isAuthenticated = userId.HasValue,
                userId = userId,
                role = role,
                teacherId = teachId
            });
        }
    }
}