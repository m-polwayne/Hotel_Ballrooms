using BallroomManager.Api.Models;

namespace BallroomManager.Api.Services
{
    public interface IBallroomService
    {
        Task<IEnumerable<Ballroom>> GetAllBallroomsAsync();
        Task<Ballroom> GetBallroomByIdAsync(int id);
        Task<Ballroom> CreateBallroomAsync(Ballroom ballroom, IFormFile imageFile);
        Task<Ballroom> UpdateBallroomAsync(int id, Ballroom ballroom, IFormFile imageFile);
        Task DeleteBallroomAsync(int id);
    }
}