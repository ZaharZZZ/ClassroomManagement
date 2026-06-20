// Models/Entities/Booking.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("bookings")]
    public class Booking : BaseEntity
    {
        [Column("teacher_id")]
        [Display(Name = "Преподаватель")]
        public int TeacherId { get; set; }

        [Column("classroom_id")]
        [Display(Name = "Кабинет")]
        public int? ClassroomId { get; set; }

        [Column("requested_room_type_id")]
        [Display(Name = "Желаемый тип аудитории")]
        public int? RequestedRoomTypeId { get; set; }

        [Column("requested_capacity")]
        [Display(Name = "Требуемая вместимость")]
        public int? RequestedCapacity { get; set; }

        [Column("preferred_building_id")]
        [Display(Name = "Предпочтительный корпус")]
        public int? PreferredBuildingId { get; set; }

        [Column("preferred_equipment")]
        [Display(Name = "Желаемое оборудование")]
        public string? PreferredEquipment { get; set; }

        [Required]
        [Column("event_date")]
        [Display(Name = "Дата")]
        public DateOnly EventDate { get; set; }

        [Required]
        [Column("start_time")]
        [Display(Name = "Начало")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [Column("end_time")]
        [Display(Name = "Конец")]
        public TimeOnly EndTime { get; set; }

        [Column("pair_number")]
        [Display(Name = "Номер пары")]
        public int? PairNumber { get; set; }

        [MaxLength(500)]
        [Column("purpose")]
        [Display(Name = "Цель резервирования")]
        public string? Purpose { get; set; }

        [MaxLength(50)]
        [Column("periodicity")]
        [Display(Name = "Периодичность")]
        public string Periodicity { get; set; } = "once";

        [Column("period_end_date")]
        [Display(Name = "Дата окончания периода")]
        public DateOnly? PeriodEndDate { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("approved_by")]
        public int? ApprovedBy { get; set; }

        [Column("approved_room_id")]
        [Display(Name = "Назначенная аудитория")]
        public int? ApprovedRoomId { get; set; }

        [MaxLength(500)]
        [Column("comment")]
        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }

        [Column("is_urgent")]
        [Display(Name = "Срочная заявка")]
        public bool IsUrgent { get; set; } = false;

        [Column("created_at")]
        [Display(Name = "Дата подачи")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        [Display(Name = "Дата изменения")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Навигационные свойства
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;

        [ForeignKey("ClassroomId")]
        public Classroom? Classroom { get; set; }

        [ForeignKey("RequestedRoomTypeId")]
        public RoomType? RequestedRoomType { get; set; }

        [ForeignKey("PreferredBuildingId")]
        public Building? PreferredBuilding { get; set; }

        [ForeignKey("StatusId")]
        public BookingStatus Status { get; set; } = null!;

        [ForeignKey("ApprovedBy")]
        public User? Approver { get; set; }

        [ForeignKey("ApprovedRoomId")]
        public Classroom? ApprovedRoom { get; set; }

        public ScheduleEntry? ScheduleEntry { get; set; }
    }
}