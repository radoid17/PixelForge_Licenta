using PixelForge.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
namespace PixelForge.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string? PublisherId { get; set; }
        public PixelForgeUser? Publisher { get; set; }
        public ICollection<UserGame>? UserGames { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public bool IsDeleted { get; set; } = false;
        public string? GameFilePath { get; set; }

    }
}
