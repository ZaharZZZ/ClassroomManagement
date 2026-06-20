// Models/Entities/Classroom.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("classrooms")]
    public class Classroom : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        [Column("number")]
        [Display(Name = "Номер аудитории")]
        public string Number { get; set; } = string.Empty;

        [Column("building_id")]
        [Display(Name = "Корпус")]
        public int BuildingId { get; set; }

        [Column("floor")]
        [Display(Name = "Этаж")]
        public int Floor { get; set; }

        [Column("room_type_id")]
        [Display(Name = "Тип аудитории")]
        public int RoomTypeId { get; set; }

        [Column("capacity")]
        [Display(Name = "Вместимость")]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        [Column("condition_id")]
        [Display(Name = "Состояние")]
        public int ConditionId { get; set; }

        [Column("department_id")]
        [Display(Name = "Отделение")]
        public int? DepartmentId { get; set; }

        [MaxLength(200)]
        [Column("long_term_booking_by")]
        [Display(Name = "Закреплена за")]
        public string? LongTermBookingBy { get; set; }

        [Column("long_term_until")]
        [Display(Name = "Закреплена до")]
        public DateTime? LongTermUntil { get; set; }

        [Column("equipment_note")]
        [Display(Name = "Оборудование")]
        public string? EquipmentNote { get; set; }

        // Навигационные свойства
        [ForeignKey("BuildingId")]
        public Building Building { get; set; } = null!;

        [ForeignKey("RoomTypeId")]
        public RoomType RoomType { get; set; } = null!;

        [ForeignKey("ConditionId")]
        public RoomCondition Condition { get; set; } = null!;

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public ICollection<ClassroomEquipment> ClassroomEquipments { get; set; }
            = new List<ClassroomEquipment>();

        public ICollection<ScheduleEntry> ScheduleEntries { get; set; }
            = new List<ScheduleEntry>();
    }
}