using System.ComponentModel.DataAnnotations;
namespace PixelForge.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public ICollection<UserGame>? UserGames { get; set; }
    }
}
