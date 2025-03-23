using Azure.Storage.Blobs;
using BallroomManager.Api.Data;
using BallroomManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BallroomManager.Api.Services
{
    public class BallroomService : IBallroomService
    {
    private readonly BallroomDbContext _context;
     private readonly BlobServiceClient _blobServiceClient;
     private readonly string _containerName = string.Empty; 

     public BallroomService(BallroomDbContext context, BlobServiceClient blobServiceClient, IConfiguration configuration)
     {
         _context = context;
         _blobServiceClient = blobServiceClient;

         _containerName = configuration.GetSection("AzureBlobStorage:ContainerName").Value;
         if (string.IsNullOrEmpty(_containerName))
         {
             throw new InvalidOperationException("AzureBlobStorage:ContainerName is missing from configuration.");
         }
     }

        public async Task<IEnumerable<Ballroom>> GetAllBallroomsAsync()
        {
            return await _context.Ballrooms.ToListAsync();
        }

        public async Task<Ballroom> GetBallroomByIdAsync(int id)
        {
            return await _context.Ballrooms.FindAsync(id);
        }

       public async Task<Ballroom> CreateBallroomAsync(Ballroom ballroom, IFormFile imageFile)
        {
             if (imageFile != null && imageFile.Length > 0)
            {
                ballroom.ImageUrl = await UploadImageAsync(imageFile);
            }

            _context.Ballrooms.Add(ballroom);
            await _context.SaveChangesAsync();
            return ballroom;
        }


        public async Task<Ballroom> UpdateBallroomAsync(int id, Ballroom ballroom, IFormFile imageFile)
        {
            var existingBallroom = await _context.Ballrooms.FindAsync(id);
            if (existingBallroom == null)
            {
                return null; 
            }

            
            existingBallroom.Name = ballroom.Name;
            existingBallroom.Description = ballroom.Description;
            existingBallroom.Capacity = ballroom.Capacity;
            existingBallroom.IsAvailable = ballroom.IsAvailable;


            if (imageFile != null && imageFile.Length > 0)
            {
                
                if (!string.IsNullOrEmpty(existingBallroom.ImageUrl))
                {
                    await DeleteImageAsync(existingBallroom.ImageUrl);
                }

                existingBallroom.ImageUrl = await UploadImageAsync(imageFile);
            }


            await _context.SaveChangesAsync();
            return existingBallroom;
        }
         public async Task DeleteBallroomAsync(int id)
        {
            var ballroom = await _context.Ballrooms.FindAsync(id);
            if (ballroom != null)
            {
                 if (!string.IsNullOrEmpty(ballroom.ImageUrl))
                {
                    await DeleteImageAsync(ballroom.ImageUrl);
                }
                _context.Ballrooms.Remove(ballroom);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();  
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true); 
            }

            return blobClient.Uri.AbsoluteUri; 
        }

         private async Task DeleteImageAsync(string imageUrl)
        {
            // Extract the blob name from the URL.  More robust way.
            Uri uri = new Uri(imageUrl);
            string blobName = uri.Segments.Last(); //Gets the last segment of the URL, which should be the blobname.

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}