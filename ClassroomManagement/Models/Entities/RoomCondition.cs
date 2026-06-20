// Models/Entities/RoomCondition.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("room_conditions")]
    public class RoomCondition : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        [Column("name")]
        [Display(Name = "Состояние")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    }
}