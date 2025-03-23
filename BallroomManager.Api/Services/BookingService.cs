using BallroomManager.Api.Data;
using BallroomManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BallroomManager.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingResponse>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Ballroom)
                .ToListAsync();

            return bookings.Select(b => new BookingResponse
            {
                Id = b.Id,
                BallroomId = b.BallroomId,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                EventDate = b.EventDate,
                EventType = b.EventType,
                GuestCount = b.GuestCount,
                SpecialRequests = b.SpecialRequests,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            });
        }

        public async Task<BookingResponse> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Ballroom)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return null;

            return new BookingResponse
            {
                Id = booking.Id,
                BallroomId = booking.BallroomId,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                EventDate = booking.EventDate,
                EventType = booking.EventType,
                GuestCount = booking.GuestCount,
                SpecialRequests = booking.SpecialRequests,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingRequest request)
        {
            var booking = new Booking
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                EventDate = request.EventDate,
                EventType = request.EventType,
                GuestCount = request.GuestCount,
                SpecialRequests = request.SpecialRequests,
                Status = "Pending",
                BallroomId = request.BallroomId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new BookingResponse
            {
                Id = booking.Id,
                BallroomId = booking.BallroomId,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                EventDate = booking.EventDate,
                EventType = booking.EventType,
                GuestCount = booking.GuestCount,
                SpecialRequests = booking.SpecialRequests,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }

        public async Task<BookingResponse> UpdateBookingStatusAsync(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return null;

            booking.Status = status;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new BookingResponse
            {
                Id = booking.Id,
                BallroomId = booking.BallroomId,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                EventDate = booking.EventDate,
                EventType = booking.EventType,
                GuestCount = booking.GuestCount,
                SpecialRequests = booking.SpecialRequests,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }
    }
} 