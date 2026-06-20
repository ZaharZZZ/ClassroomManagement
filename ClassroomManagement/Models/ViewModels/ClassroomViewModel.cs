// Models/ViewModels/ClassroomViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClassroomManagement.Models.ViewModels
{
    public class ClassroomFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите номер аудитории")]
        [MaxLength(20)]
        [Display(Name = "Номер аудитории")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите корпус")]
        [Display(Name = "Корпус")]
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "Укажите этаж")]
        [Range(1, 20)]
        [Display(Name = "Этаж")]
        public int Floor { get; set; }

        [Required(ErrorMessage = "Выберите тип аудитории")]
        [Display(Name = "Тип аудитории")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Укажите вместимость")]
        [Range(1, 1000, ErrorMessage = "Вместимость от 1 до 1000")]
        [Display(Name = "Вместимость")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Выберите состояние")]
        [Display(Name = "Состояние")]
        public int ConditionId { get; set; }

        [Display(Name = "Отделение")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Закреплена за (преподаватель)")]
        public int? SelectedTeacherId { get; set; }

        [Display(Name = "Закреплена до")]
        public DateTime? LongTermUntil { get; set; }

        // Скрытое поле для совместимости
        public string? LongTermBookingBy { get; set; }

        public SelectList? TeachersList { get; set; }

        [Display(Name = "Описание оборудования")]
        public string? EquipmentNote { get; set; }

        [Display(Name = "Оборудование")]
        public List<int> SelectedEquipmentIds { get; set; } = new();

        // Выпадающие списки
        public SelectList? Buildings { get; set; }
        public SelectList? RoomTypes { get; set; }
        public SelectList? Conditions { get; set; }
        public SelectList? Departments { get; set; }
        public List<EquipmentCheckboxViewModel> EquipmentList { get; set; } = new();
    }

    public class EquipmentCheckboxViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}