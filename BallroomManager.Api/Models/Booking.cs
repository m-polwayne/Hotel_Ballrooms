using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BallroomManager.Api.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(50)]
        public string EventType { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int GuestCount { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "PENDING";

        [Required]
        public int BallroomId { get; set; }

        [ForeignKey("BallroomId")]
        public Ballroom Ballroom { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class BookingRequest
    {
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(50)]
        public string EventType { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int GuestCount { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        [Required]
        public int BallroomId { get; set; }
    }
} 