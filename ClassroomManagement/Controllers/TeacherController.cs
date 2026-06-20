// Controllers/TeacherController.cs
using ClassroomManagement.Helpers;
using ClassroomManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IScheduleService _scheduleService;

        public TeacherController(IBookingService bookingService, IScheduleService scheduleService)
        {
            _bookingService = bookingService;
            _scheduleService = scheduleService;
        }

        // Проверка доступа
        private IActionResult? CheckAccess()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
                return RedirectToAction("Login", "Account");
            if (!SessionHelper.IsTeacher(HttpContext.Session))
                return RedirectToAction("AccessDenied", "Account");
            return null;
        }

        // GET: /Teacher/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var access = CheckAccess();
            if (access != null) return access;

            var teacherId = SessionHelper.GetTeacherId(HttpContext.Session);
            if (!teacherId.HasValue)
                return RedirectToAction("Login", "Account");

            var bookings = await _bookingService.GetTeacherBookingsAsync(teacherId.Value);
            return View(bookings);

        }

        // POST: /Teacher/CancelBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var access = CheckAccess();
            if (access != null) return access;

            var teacherId = SessionHelper.GetTeacherId(HttpContext.Session);
            if (!teacherId.HasValue)
                return Json(new { success = false, message = "Ошибка идентификации" });

            var result = await _bookingService.CancelBookingAsync(bookingId, teacherId.Value);
            return Json(new { success = result.Success, message = result.Message });
        }

        // GET: /Teacher/BookingDetail/5
        public async Task<IActionResult> BookingDetail(int id)
        {
            var access = CheckAccess();
            if (access != null) return access;

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return View(booking);
        }
    }
}