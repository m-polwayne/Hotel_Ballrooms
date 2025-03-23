using BallroomManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BallroomManager.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBallroomService _ballroomService;

        public BookingService(ApplicationDbContext context, IBallroomService ballroomService)
        {
            _context = context;
            _ballroomService = ballroomService;
        }

        public async Task<IEnumerable<BookingResponse>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Ballroom)
                .OrderByDescending(b => b.EventDate)
                .ToListAsync();

            return bookings.Select(b => new BookingResponse
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                EventDate = b.EventDate,
                EventType = b.EventType,
                GuestCount = b.GuestCount,
                SpecialRequests = b.SpecialRequests,
                Status = b.Status,
                BallroomId = b.BallroomId,
                BallroomName = b.Ballroom.Name
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
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                EventDate = booking.EventDate,
                EventType = booking.EventType,
                GuestCount = booking.GuestCount,
                SpecialRequests = booking.SpecialRequests,
                Status = booking.Status,
                BallroomId = booking.BallroomId,
                BallroomName = booking.Ballroom.Name
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
                Status = "PENDING",
                BallroomId = request.BallroomId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return await GetBookingByIdAsync(booking.Id);
        }

        public async Task UpdateBookingStatusAsync(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {id} not found");

            booking.Status = status.ToUpper();
            await _context.SaveChangesAsync();
        }
    }
} 