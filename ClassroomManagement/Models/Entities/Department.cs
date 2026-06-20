// Models/Entities/Department.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("departments")]
    public class Department : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        [Column("name")]
        [Display(Name = "Наименование отделения")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("abbreviation")]
        [Display(Name = "Аббревиатура")]
        public string Abbreviation { get; set; } = string.Empty;

        [Column("head_id")]
        [Display(Name = "Заведующий")]
        public int? HeadId { get; set; }

        // Навигационные свойства
        [ForeignKey("HeadId")]
        public Teacher? Head { get; set; }

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    }
}