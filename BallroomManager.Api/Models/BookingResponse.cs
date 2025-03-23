namespace BallroomManager.Api.Models
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int GuestCount { get; set; }
        public string? SpecialRequests { get; set; }
        public string Status { get; set; } = "PENDING";
        public int BallroomId { get; set; }
        public string BallroomName { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 