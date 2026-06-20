// Models/Entities/ClassroomEquipment.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("classroom_equipment")]
    public class ClassroomEquipment
    {
        [Column("classroom_id")]
        public int ClassroomId { get; set; }

        [Column("equipment_id")]
        public int EquipmentId { get; set; }

        // Навигационные свойства
        [ForeignKey("ClassroomId")]
        public Classroom Classroom { get; set; } = null!;

        [ForeignKey("EquipmentId")]
        public Equipment Equipment { get; set; } = null!;
    }
}