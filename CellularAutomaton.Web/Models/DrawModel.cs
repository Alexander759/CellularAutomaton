using System.ComponentModel.DataAnnotations;

namespace CellularAutomaton.Web.Models
{
    public class DrawModel
    {
        [Required]
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
