// Models/Entities/Building.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("buildings")]
    public class Building : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column("name")]
        [Display(Name = "Название корпуса")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("address")]
        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    }
}