// Models/ViewModels/BookingViewModel.cs
using ClassroomManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClassroomManagement.Models.ViewModels
{
    /// <summary>
    /// ViewModel для создания заявки преподавателем
    /// </summary>
    public class CreateBookingViewModel
    {
        public int ClassroomId { get; set; }
        public string ClassroomNumber { get; set; } = string.Empty;
        public int PairNumber { get; set; }
        public string PairTime { get; set; } = string.Empty;
        public string EventDateStr { get; set; } = string.Empty;
        public string StartTimeStr { get; set; } = string.Empty;
        public string EndTimeStr { get; set; } = string.Empty;
        public bool IsUrgent { get; set; }

        // Информация об оснащении кабинета
        public int ClassroomCapacity { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public string? EquipmentNote { get; set; }
        public List<string> ClassroomEquipments { get; set; } = new();

        [Required(ErrorMessage = "Укажите цель резервирования")]
        [MaxLength(500, ErrorMessage = "Не более 500 символов")]
        [Display(Name = "Цель резервирования")]
        public string Purpose { get; set; } = string.Empty;

        [Display(Name = "Периодичность")]
        public string Periodicity { get; set; } = "once";

        [Display(Name = "Дата окончания периода")]
        public string? PeriodEndDateStr { get; set; }
        public TimeOnly EndTime { get; internal set; }
        public TimeOnly StartTime { get; internal set; }
        public DateOnly EventDate { get; internal set; }
        public DateOnly? PeriodEndDate { get; internal set; }
    }

    /// <summary>
    /// ViewModel для назначения кабинета диспетчером
    /// </summary>
    public class DispatcherCreateBookingViewModel
    {
        public int ClassroomId { get; set; }
        public string ClassroomNumber { get; set; } = string.Empty;
        public int PairNumber { get; set; }
        public string PairTime { get; set; } = string.Empty;
        public string EventDateStr { get; set; } = string.Empty;
        public string StartTimeStr { get; set; } = string.Empty;
        public string EndTimeStr { get; set; } = string.Empty;

        // Информация об оснащении кабинета
        public int ClassroomCapacity { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public string? EquipmentNote { get; set; }
        public List<string> ClassroomEquipments { get; set; } = new();

        [Required(ErrorMessage = "Выберите преподавателя")]
        [Display(Name = "Преподаватель")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Укажите цель резервирования")]
        [MaxLength(500)]
        [Display(Name = "Цель резервирования")]
        public string Purpose { get; set; } = string.Empty;

        [Display(Name = "Периодичность")]
        public string Periodicity { get; set; } = "once";

        [Display(Name = "Дата окончания периода")]
        public string? PeriodEndDateStr { get; set; }

        // Список преподавателей
        public List<SelectListItem> TeachersList { get; set; } = new();
    }

    /// <summary>
    /// ViewModel для отображения заявки в списке
    /// </summary>
    public class BookingListItemViewModel
    {
        public int Id { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string ClassroomNumber { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int? PairNumber { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusClass { get; set; } = string.Empty;
        public bool IsUrgent { get; set; }
        public string Periodicity { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Comment { get; set; }
    }

    /// <summary>
    /// ViewModel для обработки заявки диспетчером
    /// </summary>
    public class ProcessBookingViewModel
    {
        public int BookingId { get; set; }
        public BookingListItemViewModel? BookingInfo { get; set; }
        public int? SelectedClassroomId { get; set; }
        public string? Comment { get; set; }
        public SelectList? AvailableClassrooms { get; set; }
    }
}