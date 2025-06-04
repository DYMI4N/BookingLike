using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookingLike.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal TotalPrice { get; set; } 
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
