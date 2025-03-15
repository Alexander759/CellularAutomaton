using CellularAutomaton.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Utilities;

namespace CellularAutomaton.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatesOfSimulationController : ControllerBase
    {
        const int NUMBEROFFRAMES = 20;
        private readonly IWebHostEnvironment _env;

        public StatesOfSimulationController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("sendState")]
        [HttpPost("sendState")]
        public async Task<IActionResult> SendState([FromBody] StateOfSimulationViewModel? model)
        {
            try
            {

                string base64Data = model?.Image.Replace("data:image/png;base64,", "");

                if (string.IsNullOrWhiteSpace(base64Data))
                {
                    throw new ArgumentException("No image File");
                }

                byte[] imageBytes = Convert.FromBase64String(base64Data);

                string imageName = $"{Guid.NewGuid().ToString()}.png";

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", $"{imageName}");

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);


                string pathToNewImages = PNGHandler.WriteFiles(NUMBEROFFRAMES, filePath, model.WindDirection, model.Width, model.Height, model.TileSize);

                List<string> result =new List<string>();
                
                for (int i = 0; i < NUMBEROFFRAMES; i++)
                {
                    result.Add(Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(pathToNewImages, $"{i}.png"))));
                }

                System.IO.File.Delete(filePath);
                Directory.Delete(pathToNewImages, true);
                return new JsonResult(new { message = "Success", images = result });
            }
            catch (Exception ex)
            {
                // Return an error response
                return BadRequest(new { message = "Error uploading image", details = ex.Message });
            }
        }
    }
}