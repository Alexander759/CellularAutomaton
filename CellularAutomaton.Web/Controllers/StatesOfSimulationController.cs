using CellularAutomaton.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Utilities;

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

                int n = 50;

                return new JsonResult(new { message = "Success", images = PNGHandler.WriteFiles(n, base64Data, model.WindDirection, model.Width, model.Height, model.TileSize) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error uploading image", details = ex.Message });
            }
        }
    }
}