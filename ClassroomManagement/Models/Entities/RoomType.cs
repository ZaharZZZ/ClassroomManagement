// Models/Entities/RoomType.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("room_types")]
    public class RoomType : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column("name")]
        [Display(Name = "Тип аудитории")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    }
}