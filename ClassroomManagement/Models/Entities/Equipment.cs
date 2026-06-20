// Models/Entities/Equipment.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("equipment")]
    public class Equipment : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column("name")]
        [Display(Name = "Наименование оборудования")]
        public string Name { get; set; } = string.Empty;

        public ICollection<ClassroomEquipment> ClassroomEquipments { get; set; }
            = new List<ClassroomEquipment>();
    }
}