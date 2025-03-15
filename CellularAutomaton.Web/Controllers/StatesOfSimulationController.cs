using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CellularAutomaton.Web.Controllers
{
    public class ImageRequest
    {
        public string Image { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class StatesOfSimulationController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public StatesOfSimulationController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromBody] ImageRequest? imageRequest)
        {
            try
            {
                var base64Data = imageRequest.Image.Replace("data:image/png;base64,", "");

                byte[] imageBytes = Convert.FromBase64String(base64Data);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "image.png");

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                return Ok(new { message = "Image uploaded successfully" });
            }
            catch (Exception ex)
            {
                // Return an error response
                return BadRequest(new { message = "Error uploading image", details = ex.Message });
            }
        }
    }

    
}
