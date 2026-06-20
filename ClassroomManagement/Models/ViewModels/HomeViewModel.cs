using ClassroomManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClassroomManagement.Models.ViewModels
{
    /// <summary>
    /// Информация о паре (номер, время начала, время конца)
    /// </summary>
    public class PairInfo
    {
        public int Number { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string DisplayName => $"{Number}-я пара {StartTime}-{EndTime}";
    }

    /// <summary>
    /// Состояние ячейки кабинета в сетке расписания
    /// </summary>
    public enum CellStatus
    {
        Free,       // Свободен
        Booked,     // Занят (утверждённое резервирование)
        Pending,    // Есть заявка на рассмотрении
        LongTerm    // Долгосрочное закрепление
    }

    /// <summary>
    /// Данные для одной ячейки в сетке расписания
    /// </summary>
    public class ClassroomCellViewModel
    {
        public int ClassroomId { get; set; }
        public string ClassroomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public CellStatus Status { get; set; }

        // Информация для всплывающей подсказки
        public string? TeacherName { get; set; }
        public string? Purpose { get; set; }
        public string? BookingComment { get; set; }
        public bool IsUrgent { get; set; }
        public int? BookingId { get; set; }
        public string? LongTermInfo { get; set; }

        // CSS-класс для цветовой индикации
        public string CssClass => Status switch
        {
            CellStatus.Booked => "cell-booked",
            CellStatus.Pending => "cell-pending",
            CellStatus.LongTerm => "cell-longterm",
            _ => "cell-free"
        };

        public string StatusText => Status switch
        {
            CellStatus.Booked => "Занят",
            CellStatus.Pending => "Есть заявка",
            CellStatus.LongTerm => "Закреплён",
            _ => "Свободен"
        };
    }

    /// <summary>
    /// Группа кабинетов по отделению
    /// </summary>
    public class DepartmentClassroomsViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentAbbreviation { get; set; } = string.Empty;
        public int Floor { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public List<ClassroomCellViewModel> Classrooms { get; set; } = new();
    }

    /// <summary>
    /// Главная ViewModel для страницы расписания
    /// </summary>
    public class HomeViewModel
    {
        // Выбранная дата
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        // Выбранная пара (1-5)
        public int SelectedPair { get; set; } = 1;

        // Список пар с временными промежутками
        public List<PairInfo> Pairs { get; set; } = new();

        // Текущая пара (выбранная)
        public PairInfo? CurrentPair => Pairs.FirstOrDefault(p => p.Number == SelectedPair);

        // Данные по отделениям с кабинетами
        public List<DepartmentClassroomsViewModel> DepartmentGroups { get; set; } = new();

        // ----- НОВЫЕ СВОЙСТВА ДЛЯ ФИЛЬТРОВ -----
        public int? SelectedBuildingId { get; set; }
        public int? SelectedFloor { get; set; }
        public int? SelectedDepartmentId { get; set; }

        public SelectList? Buildings { get; set; }
        public SelectList? Floors { get; set; }
        public SelectList? Departments { get; set; }

        // Вспомогательные свойства для отображения
        public string DayOfWeekRu => SelectedDate.DayOfWeek switch
        {
            DayOfWeek.Monday => "Понедельник",
            DayOfWeek.Tuesday => "Вторник",
            DayOfWeek.Wednesday => "Среда",
            DayOfWeek.Thursday => "Четверг",
            DayOfWeek.Friday => "Пятница",
            DayOfWeek.Saturday => "Суббота",
            DayOfWeek.Sunday => "Воскресенье",
            _ => string.Empty
        };

        public string FormattedDate => SelectedDate.ToString("dd.MM.yyyy");
    }
}