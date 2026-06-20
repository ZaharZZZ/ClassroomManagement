// Models/Entities/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("users")]
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column("login")]
        [Display(Name = "Логин")]
        public string Login { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("role_id")]
        [Display(Name = "Роль")]
        public int RoleId { get; set; }

        [Column("teacher_id")]
        public int? TeacherId { get; set; }

        [Column("is_blocked")]
        [Display(Name = "Заблокирован")]
        public bool IsBlocked { get; set; } = false;

        [Column("created_at")]
        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("last_login")]
        [Display(Name = "Последний вход")]
        public DateTime? LastLogin { get; set; }

        // Навигационные свойства
        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }
    }
}