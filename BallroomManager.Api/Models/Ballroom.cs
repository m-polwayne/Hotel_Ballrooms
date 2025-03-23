// BallroomManager.Api/Models/Ballroom.cs
namespace BallroomManager.Api.Models
{
    public class Ballroom
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Dimesions { get; set; }
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; } // URL to the image in Blob Storage
        public bool IsAvailable { get; set; } // Example additional property

    }
}