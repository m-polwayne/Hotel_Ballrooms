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
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();
                
                // Generate a unique filename with the original extension
                var blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var blobClient = containerClient.GetBlobClient(blobName);

                Console.WriteLine($"Uploading image to: {blobClient.Uri}");

                using (var stream = imageFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                // Always return just the filename in both development and production
                // The frontend will handle constructing the full URL
                return blobName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
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

        public async Task<byte[]?> GetImageAsync(string filename)
        {
            try
            {
                // Clean up the filename in case it contains URL parts
                filename = Path.GetFileName(filename);
                Console.WriteLine($"Attempting to retrieve image: {filename}");
                
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();
                
                var blobClient = containerClient.GetBlobClient(filename);
                Console.WriteLine($"Checking if blob exists at: {blobClient.Uri}");
                
                if (!await blobClient.ExistsAsync())
                {
                    Console.WriteLine($"Blob not found: {filename}");
                    return null;
                }

                using var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;
                var bytes = memoryStream.ToArray();
                Console.WriteLine($"Successfully retrieved image: {filename}, size: {bytes.Length} bytes");
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving image: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}