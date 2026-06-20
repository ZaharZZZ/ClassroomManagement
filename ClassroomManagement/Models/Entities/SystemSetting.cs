// Models/Entities/SystemSetting.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("system_settings")]
    public class SystemSetting : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column("setting_key")]
        public string SettingKey { get; set; } = string.Empty;

        [Column("setting_value")]
        public string? SettingValue { get; set; }

        [MaxLength(255)]
        [Column("description")]
        public string? Description { get; set; }
    }
}