using PixelForge.Areas.Identity.Data;

namespace PixelForge.Models
{
    public class UserGame
    {
        public string UserId { get; set; }
        public PixelForgeUser User { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
