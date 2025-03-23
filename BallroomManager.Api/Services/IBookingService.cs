using BallroomManager.Api.Models;

namespace BallroomManager.Api.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponse>> GetAllBookingsAsync();
        Task<BookingResponse> GetBookingByIdAsync(int id);
        Task<BookingResponse> CreateBookingAsync(BookingRequest booking);
        Task UpdateBookingStatusAsync(int id, string status);
    }
} 