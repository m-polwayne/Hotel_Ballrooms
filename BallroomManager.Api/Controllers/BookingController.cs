using Microsoft.AspNetCore.Mvc;
using BallroomManager.Api.Models;
using BallroomManager.Api.Services;

namespace BallroomManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
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
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving bookings.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponse>> GetBooking(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound();

                return Ok(booking);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving the booking.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponse>> CreateBooking(BookingRequest request)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(request);
                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the booking.");
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<BookingResponse>> UpdateBookingStatus(int id, [FromBody] string status)
        {
            try
            {
                var booking = await _bookingService.UpdateBookingStatusAsync(id, status);
                if (booking == null)
                    return NotFound();

                return Ok(booking);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the booking status.");
            }
        }
    }
} 