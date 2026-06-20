// Services/BookingService.cs
using ClassroomManagement.Data;
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Models.ViewModels;
using ClassroomManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public BookingService(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<(bool Success, string Message, int BookingId)> CreateBookingAsync(
            CreateBookingViewModel model, int teacherId)
        {
            // Проверяем коллизии
            if (await HasConflictAsync(model.ClassroomId, model.EventDate,
                model.StartTime, model.EndTime))
            {
                return (false, "Кабинет уже занят на это время", 0);
            }

            // Определяем срочность: заявка на сегодня
            var isUrgent = model.EventDate == DateOnly.FromDateTime(DateTime.Today);

            // Получаем статус "Pending"
            var pendingStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.Name == "Pending");
            if (pendingStatus == null)
                return (false, "Ошибка конфигурации системы", 0);

            var booking = new Booking
            {
                TeacherId = teacherId,
                ClassroomId = model.ClassroomId,
                EventDate = model.EventDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                PairNumber = model.PairNumber,
                Purpose = model.Purpose,
                Periodicity = model.Periodicity,
                PeriodEndDate = model.PeriodEndDate,
                StatusId = pendingStatus.Id,
                IsUrgent = isUrgent,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(null, "CreateBooking",
                $"Создана заявка #{booking.Id} на кабинет {model.ClassroomId} " +
                $"на {model.EventDate} пара {model.PairNumber}");

            return (true, "Заявка успешно подана", booking.Id);
        }

        public async Task<List<BookingListItemViewModel>> GetTeacherBookingsAsync(int teacherId)
        {
            return await _context.Bookings
                .Include(b => b.Teacher)
                .Include(b => b.Classroom).ThenInclude(c => c!.Building)
                .Include(b => b.ApprovedRoom).ThenInclude(c => c!.Building)
                .Include(b => b.Status)
                .Where(b => b.TeacherId == teacherId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => MapToListItem(b))
                .ToListAsync();
        }

        public async Task<List<BookingListItemViewModel>> GetAllBookingsAsync(
            string? statusFilter = null, DateOnly? dateFrom = null, DateOnly? dateTo = null)
        {
            var query = _context.Bookings
                .Include(b => b.Teacher)
                .Include(b => b.Classroom).ThenInclude(c => c!.Building)
                .Include(b => b.ApprovedRoom).ThenInclude(c => c!.Building)
                .Include(b => b.Status)
                .AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
                query = query.Where(b => b.Status.Name == statusFilter);

            if (dateFrom.HasValue)
                query = query.Where(b => b.EventDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(b => b.EventDate <= dateTo.Value);

            return await query
                .OrderByDescending(b => b.IsUrgent)
                .ThenBy(b => b.EventDate)
                .Select(b => MapToListItem(b))
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> ApproveBookingAsync(
            int bookingId, int classroomId, int dispatcherUserId, string? comment = null)
        {
            var booking = await _context.Bookings
                .Include(b => b.Status)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return (false, "Заявка не найдена");

            // Проверяем коллизии для выбранного кабинета
            if (await HasConflictAsync(classroomId, booking.EventDate,
                booking.StartTime, booking.EndTime, bookingId))
                return (false, "Выбранный кабинет уже занят на это время");

            var approvedStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.Name == "Approved");
            if (approvedStatus == null)
                return (false, "Ошибка конфигурации");

            // Обновляем заявку
            booking.StatusId = approvedStatus.Id;
            booking.ApprovedRoomId = classroomId;
            booking.ApprovedBy = dispatcherUserId;
            booking.Comment = comment;
            booking.UpdatedAt = DateTime.Now;

            // Создаём запись в расписании
            var scheduleEntry = new ScheduleEntry
            {
                BookingId = bookingId,
                ClassroomId = classroomId,
                TeacherId = booking.TeacherId,
                EventDate = booking.EventDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                PairNumber = booking.PairNumber,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.ScheduleEntries.Add(scheduleEntry);

            // Если периодическая заявка — создаём записи на все даты
            if (booking.Periodicity == "weekly" && booking.PeriodEndDate.HasValue)
            {
                var nextDate = booking.EventDate.AddDays(7);
                while (nextDate <= booking.PeriodEndDate.Value)
                {
                    _context.ScheduleEntries.Add(new ScheduleEntry
                    {
                        BookingId = bookingId,
                        ClassroomId = classroomId,
                        TeacherId = booking.TeacherId,
                        EventDate = nextDate,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        PairNumber = booking.PairNumber,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                    nextDate = nextDate.AddDays(7);
                }
            }

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(dispatcherUserId, "ApproveBooking",
                $"Заявка #{bookingId} утверждена, кабинет {classroomId}");

            return (true, "Заявка утверждена");
        }

        public async Task<(bool Success, string Message)> RejectBookingAsync(
            int bookingId, int dispatcherUserId, string reason)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return (false, "Заявка не найдена");

            var rejectedStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.Name == "Rejected");
            if (rejectedStatus == null)
                return (false, "Ошибка конфигурации");

            booking.StatusId = rejectedStatus.Id;
            booking.ApprovedBy = dispatcherUserId;
            booking.Comment = reason;
            booking.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(dispatcherUserId, "RejectBooking",
                $"Заявка #{bookingId} отклонена. Причина: {reason}");

            return (true, "Заявка отклонена");
        }

        public async Task<(bool Success, string Message)> CancelBookingAsync(
            int bookingId, int teacherId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Status)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.TeacherId == teacherId);

            if (booking == null)
                return (false, "Заявка не найдена");

            var cancelledStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.Name == "Cancelled");
            if (cancelledStatus == null)
                return (false, "Ошибка конфигурации");

            booking.StatusId = cancelledStatus.Id;
            booking.UpdatedAt = DateTime.Now;

            // Удаляем запись из расписания если была утверждена
            var scheduleEntry = await _context.ScheduleEntries
                .FirstOrDefaultAsync(s => s.BookingId == bookingId);
            if (scheduleEntry != null)
                _context.ScheduleEntries.Remove(scheduleEntry);

            await _context.SaveChangesAsync();
            return (true, "Заявка отменена");
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Teacher)
                .Include(b => b.Classroom).ThenInclude(c => c!.Building)
                .Include(b => b.ApprovedRoom).ThenInclude(c => c!.Building)
                .Include(b => b.Status)
                .Include(b => b.Approver)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<bool> HasConflictAsync(
            int classroomId, DateOnly date, TimeOnly startTime, TimeOnly endTime,
            int? excludeBookingId = null)
        {
            // Проверяем расписание
            var scheduleConflict = await _context.ScheduleEntries.AnyAsync(s =>
                s.ClassroomId == classroomId &&
                s.EventDate == date &&
                s.StartTime < endTime &&
                s.EndTime > startTime &&
                (excludeBookingId == null || s.BookingId != excludeBookingId));

            if (scheduleConflict) return true;

            // Проверяем заявки на рассмотрении
            var bookingConflict = await _context.Bookings.AnyAsync(b =>
                (b.ClassroomId == classroomId || b.ApprovedRoomId == classroomId) &&
                b.EventDate == date &&
                b.StartTime < endTime &&
                b.EndTime > startTime &&
                b.Status.Name == "Pending" &&
                (excludeBookingId == null || b.Id != excludeBookingId));

            return bookingConflict;
        }

        public async Task<List<Classroom>> GetAvailableClassroomsAsync(
    DateOnly date, TimeOnly startTime, TimeOnly endTime,
    int? buildingId = null, int? roomTypeId = null, int? minCapacity = null,
    int? excludeBookingId = null)
        {
            // Кабинеты, занятые в расписании (утверждённые)
            var bookedIds = await _context.ScheduleEntries
                .Where(s => s.EventDate == date &&
                            s.StartTime < endTime &&
                            s.EndTime > startTime)
                .Select(s => s.ClassroomId)
                .ToListAsync();

            // Кабинеты с заявками на рассмотрении, исключая текущую заявку
            var pendingQuery = _context.Bookings
                .Where(b => b.EventDate == date &&
                            b.StartTime < endTime &&
                            b.EndTime > startTime &&
                            b.Status.Name == "Pending");
            if (excludeBookingId.HasValue)
                pendingQuery = pendingQuery.Where(b => b.Id != excludeBookingId.Value);

            var pendingIds = await pendingQuery
                .Select(b => b.ClassroomId ?? 0)
                .ToListAsync();

            var occupiedIds = bookedIds.Union(pendingIds).ToList();

            var query = _context.Classrooms
                .Include(c => c.Building)
                .Include(c => c.RoomType)
                .Include(c => c.Condition)
                .Where(c => !occupiedIds.Contains(c.Id) && c.Condition.Name == "Исправна");

            if (buildingId.HasValue)
                query = query.Where(c => c.BuildingId == buildingId.Value);

            if (roomTypeId.HasValue)
                query = query.Where(c => c.RoomTypeId == roomTypeId.Value);

            if (minCapacity.HasValue)
                query = query.Where(c => c.Capacity >= minCapacity.Value);

            return await query.OrderBy(c => c.Number).ToListAsync();
        }

        // Вспомогательный метод маппинга
        private static BookingListItemViewModel MapToListItem(Booking b)
        {
            var displayRoom = b.ApprovedRoom ?? b.Classroom;
            return new BookingListItemViewModel
            {
                Id = b.Id,
                TeacherName = b.Teacher?.FullName ?? "",
                ClassroomNumber = displayRoom?.Number ?? "—",
                BuildingName = displayRoom?.Building?.Name ?? "",
                EventDate = b.EventDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                PairNumber = b.PairNumber,
                Purpose = b.Purpose ?? "",
                StatusName = b.Status?.Name ?? "",
                StatusClass = b.Status?.Name switch
                {
                    "Pending" => "badge-warning",
                    "Approved" => "badge-success",
                    "Rejected" => "badge-danger",
                    "Cancelled" => "badge-secondary",
                    _ => "badge-light"
                },
                IsUrgent = b.IsUrgent,
                Periodicity = b.Periodicity,
                CreatedAt = b.CreatedAt,
                Comment = b.Comment
            };
        }
    }
}