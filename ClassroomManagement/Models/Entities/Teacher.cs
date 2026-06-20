// Models/Entities/Teacher.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("teachers")]
    public class Teacher : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        [Column("full_name")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("academic_degree")]
        [Display(Name = "Учёная степень")]
        public string? AcademicDegree { get; set; }

        [Column("department_id")]
        [Display(Name = "Отделение")]
        public int? DepartmentId { get; set; }

        // Навигационные свойства
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public User? User { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>();
    }
}