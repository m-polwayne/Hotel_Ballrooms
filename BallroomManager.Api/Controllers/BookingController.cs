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
        private readonly IBookingService _bookingService;

        public BookingController(IBallroomService ballroomService, IBookingService bookingService)
        {
            _ballroomService = ballroomService;
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving bookings");
            }
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

                var newBooking = await _bookingService.CreateBookingAsync(booking);
                return Ok(new { message = "Booking request received successfully", booking = newBooking });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your booking");
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusRequest request)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    return NotFound("Booking not found");
                }

                await _bookingService.UpdateBookingStatusAsync(id, request.Status);
                return Ok(new { message = $"Booking status updated to {request.Status}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the booking status");
            }
        }
    }

    public class UpdateBookingStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

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