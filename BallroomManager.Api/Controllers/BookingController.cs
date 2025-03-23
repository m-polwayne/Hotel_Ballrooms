using Microsoft.AspNetCore.Mvc;
using BallroomManager.Api.Models;
using BallroomManager.Api.Services;

namespace BallroomManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBallroomService _ballroomService;

        public BookingController(IBallroomService ballroomService)
        {
            _ballroomService = ballroomService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest booking)
        {
            try
            {
                // Validate ballroom exists and has capacity
                var ballroom = await _ballroomService.GetBallroomByIdAsync(booking.BallroomId);
                if (ballroom == null)
                {
                    return NotFound("Ballroom not found");
                }

                if (booking.GuestCount > ballroom.Capacity)
                {
                    return BadRequest($"Guest count exceeds ballroom capacity of {ballroom.Capacity}");
                }

                // TODO: Add actual booking logic here
                // For now, just return success
                return Ok(new { message = "Booking request received successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your booking");
            }
        }
    }

    public class BookingRequest
    {
        public int BallroomId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int GuestCount { get; set; }
        public string? SpecialRequests { get; set; }
    }
} 