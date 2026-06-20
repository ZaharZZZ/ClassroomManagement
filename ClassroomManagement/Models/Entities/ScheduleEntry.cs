// Models/Entities/ScheduleEntry.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("schedule")]
    public class ScheduleEntry : BaseEntity
    {
        [Column("booking_id")]
        public int? BookingId { get; set; }

        [Required]
        [Column("classroom_id")]
        public int ClassroomId { get; set; }

        [Required]
        [Column("teacher_id")]
        public int TeacherId { get; set; }

        [Required]
        [Column("event_date")]
        public DateOnly EventDate { get; set; }

        [Required]
        [Column("start_time")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [Column("end_time")]
        public TimeOnly EndTime { get; set; }

        [Column("pair_number")]
        public int? PairNumber { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Навигационные свойства
        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [ForeignKey("ClassroomId")]
        public Classroom Classroom { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;
    }
}