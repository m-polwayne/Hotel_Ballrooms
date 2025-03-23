using BallroomManager.Api.Models;

namespace BallroomManager.Api.Services
{
    public interface IBallroomService
    {
        Task<IEnumerable<Ballroom>> GetAllBallroomsAsync();
        Task<Ballroom?> GetBallroomByIdAsync(int id);
        Task<Ballroom> CreateBallroomAsync(Ballroom ballroom, IFormFile? image = null);
        Task<Ballroom?> UpdateBallroomAsync(int id, Ballroom ballroom, IFormFile? image = null);
        Task DeleteBallroomAsync(int id);
        Task<byte[]?> GetImageAsync(string filename);
    }
}