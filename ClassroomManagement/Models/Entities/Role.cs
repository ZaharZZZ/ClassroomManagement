// Models/Entities/Role.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("roles")]
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        // Навигационное свойство
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}