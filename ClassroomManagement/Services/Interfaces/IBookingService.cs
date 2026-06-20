// Services/Interfaces/IBookingService.cs
using ClassroomManagement.Models.Entities;
using ClassroomManagement.Models.ViewModels;

namespace ClassroomManagement.Services.Interfaces
{
    public interface IBookingService
    {
        /// <summary>
        /// Создание новой заявки от преподавателя
        /// </summary>
        Task<(bool Success, string Message, int BookingId)> CreateBookingAsync(
            CreateBookingViewModel model, int teacherId);

        /// <summary>
        /// Получение заявок преподавателя
        /// </summary>
        Task<List<BookingListItemViewModel>> GetTeacherBookingsAsync(int teacherId);

        /// <summary>
        /// Получение всех заявок для диспетчера (с фильтрацией)
        /// </summary>
        Task<List<BookingListItemViewModel>> GetAllBookingsAsync(
            string? statusFilter = null, DateOnly? dateFrom = null, DateOnly? dateTo = null);

        /// <summary>
        /// Утверждение заявки диспетчером
        /// </summary>
        Task<(bool Success, string Message)> ApproveBookingAsync(
            int bookingId, int classroomId, int dispatcherUserId, string? comment = null);

        /// <summary>
        /// Отклонение заявки диспетчером
        /// </summary>
        Task<(bool Success, string Message)> RejectBookingAsync(
            int bookingId, int dispatcherUserId, string reason);

        /// <summary>
        /// Отмена заявки преподавателем
        /// </summary>
        Task<(bool Success, string Message)> CancelBookingAsync(
            int bookingId, int teacherId);

        /// <summary>
        /// Получение детальной информации о заявке
        /// </summary>
        Task<Booking?> GetBookingByIdAsync(int bookingId);

        /// <summary>
        /// Проверка коллизий (занятости кабинета)
        /// </summary>
        Task<bool> HasConflictAsync(
            int classroomId, DateOnly date, TimeOnly startTime, TimeOnly endTime,
            int? excludeBookingId = null);

        /// <summary>
        /// Получение свободных кабинетов на указанное время
        /// </summary>
        Task<List<Classroom>> GetAvailableClassroomsAsync(
    DateOnly date, TimeOnly startTime, TimeOnly endTime,
    int? buildingId = null, int? roomTypeId = null, int? minCapacity = null,
    int? excludeBookingId = null); // новый параметр


    }
}