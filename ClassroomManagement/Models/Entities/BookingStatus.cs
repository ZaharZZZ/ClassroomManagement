// Models/Entities/BookingStatus.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomManagement.Models.Entities
{
    [Table("booking_statuses")]
    public class BookingStatus : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        [Column("name")]
        [Display(Name = "Статус")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}