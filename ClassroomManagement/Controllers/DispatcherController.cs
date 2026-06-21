// Controllers/DispatcherController.cs
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
    public class DispatcherController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IScheduleService _scheduleService;
        private readonly IAuditService _auditService;
        private readonly ApplicationDbContext _context;

        public DispatcherController(
            IBookingService bookingService,
            IScheduleService scheduleService,
            IAuditService auditService,
            ApplicationDbContext context)
        {
            _bookingService  = bookingService;
            _scheduleService = scheduleService;
            _auditService    = auditService;
            _context         = context;
        }

        // -----------------------------------------------
        // Проверка доступа
        // -----------------------------------------------
        private IActionResult? CheckAccess()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
                return RedirectToAction("Login", "Account");
            if (!SessionHelper.IsDispatcher(HttpContext.Session) &&
                !SessionHelper.IsAdmin(HttpContext.Session))
                return RedirectToAction("AccessDenied", "Account");
            return null;
        }

        // -----------------------------------------------
        // GET: /Dispatcher/Index
        // -----------------------------------------------
        public async Task<IActionResult> Index(
            string? status = "Pending",
            string? dateFrom = null,
            string? dateTo = null)
        {
            var access = CheckAccess();
            if (access != null) return access;

            DateOnly? from = null, to = null;
            if (DateOnly.TryParse(dateFrom, out var f)) from = f;
            if (DateOnly.TryParse(dateTo,   out var t)) to   = t;

            var bookings = await _bookingService
                .GetAllBookingsAsync(status, from, to);

            ViewBag.StatusFilter  = status;
            ViewBag.DateFrom      = dateFrom;
            ViewBag.DateTo        = dateTo;
            ViewBag.PendingCount  = (await _bookingService
                .GetAllBookingsAsync("Pending")).Count;

            return View(bookings);
        }

        // -----------------------------------------------
        // GET: /Dispatcher/ProcessBooking/5
        // -----------------------------------------------
        public async Task<IActionResult> ProcessBooking(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            var available = await _bookingService.GetAvailableClassroomsAsync(
                booking.EventDate, booking.StartTime, booking.EndTime,
                excludeBookingId: booking.Id); // ← добавляем ID текущей заявки

            var model = new ProcessBookingViewModel
            {
                BookingId = id,
                BookingInfo = new BookingListItemViewModel
                {
                    Id             = booking.Id,
                    TeacherName    = booking.Teacher.FullName,
                    ClassroomNumber = booking.Classroom?.Number ?? "—",
                    EventDate      = booking.EventDate,
                    StartTime      = booking.StartTime,
                    EndTime        = booking.EndTime,
                    PairNumber     = booking.PairNumber,
                    Purpose        = booking.Purpose ?? "",
                    StatusName     = booking.Status.Name,
                    IsUrgent       = booking.IsUrgent,
                    Periodicity    = booking.Periodicity
                },
                AvailableClassrooms = new SelectList(
                    available.Select(c => new
                    {
                        c.Id,
                        Name = $"Каб. {c.Number} ({c.Building.Name}, {c.Capacity} мест)"
                    }),
                    "Id", "Name", booking.ClassroomId)
            };

            return View(model);
        }

        // -----------------------------------------------
        // POST: /Dispatcher/ApproveBooking
        // -----------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBooking(
            ProcessBookingViewModel model)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (!model.SelectedClassroomId.HasValue)
            {
                TempData["Error"] = "Выберите кабинет для утверждения";
                return RedirectToAction("ProcessBooking", new { id = model.BookingId });
            }

            var userId = SessionHelper.GetUserId(HttpContext.Session)!.Value;
            var result = await _bookingService.ApproveBookingAsync(
                model.BookingId,
                model.SelectedClassroomId.Value,
                userId,
                model.Comment);

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Index");
        }

        // -----------------------------------------------
        // POST: /Dispatcher/RejectBooking
        // -----------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBooking(int bookingId, string reason)
        {
            var access = CheckAccess();
            if (access != null) return access;

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "Укажите причину отклонения";
                return RedirectToAction("ProcessBooking", new { id = bookingId });
            }

            var userId = SessionHelper.GetUserId(HttpContext.Session)!.Value;
            var result = await _bookingService
                .RejectBookingAsync(bookingId, userId, reason);

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Index");
        }

        // -----------------------------------------------
        // GET: /Dispatcher/Classrooms
        // -----------------------------------------------
        public async Task<IActionResult> Classrooms()
        {
            var access = CheckAccess();
            if (access != null) return access;

            var classrooms = await _context.Classrooms
                .Include(c => c.Building)
                .Include(c => c.RoomType)
                .Include(c => c.Condition)
                .Include(c => c.Department)
                .Include(c => c.ClassroomEquipments)
                    .ThenInclude(ce => ce.Equipment)
                .OrderBy(c => c.Building.Name)
                .ThenBy(c => c.Floor)
                .ThenBy(c => c.Number)
                .ToListAsync();

            return View(classrooms);
        }

        // ===============================================
        // НАЗНАЧЕНИЕ КАБИНЕТА ДИСПЕТЧЕРОМ
        // ===============================================

        // -----------------------------------------------
        // GET: /Dispatcher/AssignForm  (AJAX)
        // Загружает форму назначения кабинета преподавателю
        // -----------------------------------------------
        [HttpGet]
        public async Task<IActionResult> AssignForm(
            int classroomId, string date, int pair)
        {
            // Проверка доступа — возвращаем HTML с ошибкой
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
                return Content("<div class='alert alert-danger'>Необходима авторизация</div>");

            if (!SessionHelper.IsDispatcher(HttpContext.Session) &&
                !SessionHelper.IsAdmin(HttpContext.Session))
                return Content("<div class='alert alert-danger'>Доступ запрещён</div>");

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

            if (classroom == null)
                return Content("<div class='alert alert-danger'>Кабинет не найден</div>");

            // Загружаем список преподавателей
            var teachers = await _context.Teachers
                .Include(t => t.Department)
                .OrderBy(t => t.FullName)
                .ToListAsync();

            var model = new DispatcherCreateBookingViewModel
            {
                ClassroomId         = classroomId,
                ClassroomNumber     = classroom.Number,
                PairNumber          = pair,
                PairTime            = selectedPair.DisplayName,
                EventDateStr        = dateOnly.ToString("yyyy-MM-dd"),
                StartTimeStr        = selectedPair.StartTime,
                EndTimeStr          = selectedPair.EndTime,
                ClassroomCapacity   = classroom.Capacity,
                RoomTypeName        = classroom.RoomType?.Name ?? "",
                EquipmentNote       = classroom.EquipmentNote,
                ClassroomEquipments = classroom.ClassroomEquipments
                    .Select(ce => ce.Equipment.Name).ToList(),
                TeachersList = teachers.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text  = t.Department != null
                        ? $"{t.FullName} ({t.Department.Abbreviation})"
                        : t.FullName
                }).ToList()
            };

            return PartialView("_AssignForm", model);
        }

        // -----------------------------------------------
        // POST: /Dispatcher/AssignClassroom  (AJAX)
        // Назначает кабинет преподавателю
        // -----------------------------------------------
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AssignClassroom(
            int classroomId,
            int teacherId,
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

            if (!SessionHelper.IsDispatcher(HttpContext.Session) &&
                !SessionHelper.IsAdmin(HttpContext.Session))
                return Json(new { success = false, message = "Доступ запрещён" });

            // Валидация входных данных
            if (teacherId <= 0)
                return Json(new { success = false, message = "Выберите преподавателя" });

            if (string.IsNullOrWhiteSpace(purpose))
                return Json(new { success = false, message = "Укажите цель резервирования" });

            if (!DateOnly.TryParse(eventDateStr, out var eventDate))
                return Json(new { success = false, message = "Неверный формат даты" });

            if (eventDate.DayOfWeek == DayOfWeek.Sunday)
                return Json(new { success = false, message = "Резервирование на воскресенье запрещено" });

            if (!TimeOnly.TryParse(startTimeStr, out var startTime))
                return Json(new { success = false, message = "Неверный формат времени начала" });

            if (!TimeOnly.TryParse(endTimeStr, out var endTime))
                return Json(new { success = false, message = "Неверный формат времени окончания" });

            DateOnly? periodEndDate = null;
            if (!string.IsNullOrEmpty(periodEndDateStr) &&
                DateOnly.TryParse(periodEndDateStr, out var ped))
                periodEndDate = ped;

            // Проверяем коллизии
            var hasConflict = await _bookingService.HasConflictAsync(
                classroomId, eventDate, startTime, endTime);
            if (hasConflict)
                return Json(new
                {
                    success = false,
                    message = "Кабинет уже занят на это время. Выберите другой."
                });

            // Проверяем существование преподавателя
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null)
                return Json(new { success = false, message = "Преподаватель не найден" });

            // Проверяем существование кабинета
            var classroom = await _context.Classrooms.FindAsync(classroomId);
            if (classroom == null)
                return Json(new { success = false, message = "Кабинет не найден" });

            // Получаем статус "Approved" — диспетчер сразу утверждает
            var approvedStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.Name == "Approved");
            if (approvedStatus == null)
                return Json(new { success = false, message = "Ошибка конфигурации системы" });

            var dispatcherUserId = SessionHelper.GetUserId(HttpContext.Session)!.Value;
            var isUrgent = eventDate == DateOnly.FromDateTime(DateTime.Today);

            // Создаём заявку сразу со статусом "Утверждена"
            var booking = new Booking
            {
                TeacherId      = teacherId,
                ClassroomId    = classroomId,
                ApprovedRoomId = classroomId,
                EventDate      = eventDate,
                StartTime      = startTime,
                EndTime        = endTime,
                PairNumber     = pairNumber,
                Purpose        = purpose.Trim(),
                Periodicity    = periodicity,
                PeriodEndDate  = periodEndDate,
                StatusId       = approvedStatus.Id,
                ApprovedBy     = dispatcherUserId,
                IsUrgent       = isUrgent,
                CreatedAt      = DateTime.Now,
                UpdatedAt      = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Создаём запись в расписании
            _context.ScheduleEntries.Add(new ScheduleEntry
            {
                BookingId   = booking.Id,
                ClassroomId = classroomId,
                TeacherId   = teacherId,
                EventDate   = eventDate,
                StartTime   = startTime,
                EndTime     = endTime,
                PairNumber  = pairNumber,
                CreatedAt   = DateTime.Now,
                UpdatedAt   = DateTime.Now
            });

            // Если еженедельно — создаём записи на все даты
            if (periodicity == "weekly" && periodEndDate.HasValue)
            {
                var nextDate = eventDate.AddDays(7);
                while (nextDate <= periodEndDate.Value)
                {
                    // Проверяем коллизии для каждой даты
                    var conflict = await _bookingService.HasConflictAsync(
                        classroomId, nextDate, startTime, endTime);
                    if (!conflict)
                    {
                        _context.ScheduleEntries.Add(new ScheduleEntry
                        {
                            BookingId   = booking.Id,
                            ClassroomId = classroomId,
                            TeacherId   = teacherId,
                            EventDate   = nextDate,
                            StartTime   = startTime,
                            EndTime     = endTime,
                            PairNumber  = pairNumber,
                            CreatedAt   = DateTime.Now,
                            UpdatedAt   = DateTime.Now
                        });
                    }
                    nextDate = nextDate.AddDays(7);
                }
            }

            await _context.SaveChangesAsync();

            // Логируем действие
            await _auditService.LogAsync(
                dispatcherUserId,
                "AssignClassroom",
                $"Диспетчер назначил кабинет {classroom.Number} " +
                $"преподавателю {teacher.FullName} на {eventDate:dd.MM.yyyy} " +
                $"пара {pairNumber}",
                HttpContext.Connection.RemoteIpAddress?.ToString());

            return Json(new
            {
                success = true,
                message = $"Кабинет {classroom.Number} успешно назначен " +
                          $"преподавателю {teacher.FullName} " +
                          $"на {eventDate:dd.MM.yyyy}"
            });
        }
    }
}