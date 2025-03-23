using BallroomManager.Api.Models;
using BallroomManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BallroomManager.Api.Controllers
{
    /// <summary>
    /// Controller for managing ballrooms
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BallroomsController : ControllerBase
    {
        private readonly IBallroomService _ballroomService;

        public BallroomsController(IBallroomService ballroomService)
        {
            _ballroomService = ballroomService;
        }

        /// <summary>
        /// Gets all ballrooms
        /// </summary>
        /// <returns>A list of all ballrooms</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ballroom>>> GetBallrooms()
        {
            var ballrooms = await _ballroomService.GetAllBallroomsAsync();
            return Ok(ballrooms);
        }

        /// <summary>
        /// Gets a specific ballroom by id
        /// </summary>
        /// <param name="id">The ID of the ballroom to retrieve</param>
        /// <returns>The requested ballroom</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Ballroom>> GetBallroom(int id)
        {
            var ballroom = await _ballroomService.GetBallroomByIdAsync(id);
            if (ballroom == null)
            {
                return NotFound();
            }
            return Ok(ballroom);
        }

        /// <summary>
        /// Gets a ballroom image by filename
        /// </summary>
        /// <param name="filename">The filename of the image to retrieve</param>
        /// <returns>The image file</returns>
        [HttpGet("images/{filename}")]
        public async Task<IActionResult> GetImage(string filename)
        {
            try
            {
                var imageBytes = await _ballroomService.GetImageAsync(filename);
                if (imageBytes == null)
                {
                    return NotFound();
                }

                // Determine content type based on file extension
                var extension = Path.GetExtension(filename).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    _ => "application/octet-stream"
                };

                return File(imageBytes, contentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Creates a new ballroom
        /// </summary>
        /// <param name="ballroom">The ballroom to create</param>
        /// <param name="image">Optional image file for the ballroom</param>
        /// <returns>The created ballroom</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Ballroom>> CreateBallroom([FromForm] Ballroom ballroom, IFormFile? image = null)
        {
            var createdBallroom = await _ballroomService.CreateBallroomAsync(ballroom, image);
            return CreatedAtAction(nameof(GetBallroom), new { id = createdBallroom.Id }, createdBallroom);
        }

        /// <summary>
        /// Updates an existing ballroom
        /// </summary>
        /// <param name="id">The ID of the ballroom to update</param>
        /// <param name="ballroom">The updated ballroom data</param>
        /// <param name="image">Optional new image file for the ballroom</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateBallroom(int id, [FromForm] Ballroom ballroom, IFormFile? image = null)
        {
            if (id != ballroom.Id)
            {
                return BadRequest();
            }

            var updatedBallroom = await _ballroomService.UpdateBallroomAsync(id, ballroom, image);

            if (updatedBallroom == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific ballroom
        /// </summary>
        /// <param name="id">The ID of the ballroom to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBallroom(int id)
        {
            await _ballroomService.DeleteBallroomAsync(id);
            return NoContent();
        }
    }
}