using PixelForge.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
namespace PixelForge.Models
{
    public enum AgeRating
    {
        E,
        E10,
        T,
        M,
        AO
    }

    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string Genre { get; set; } = string.Empty;

        [Display(Name = "Age Rating")]
        public AgeRating AgeRating { get; set; }
        public string? PublisherId { get; set; }
        public PixelForgeUser? Publisher { get; set; }
        public ICollection<UserGame>? UserGames { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public bool IsDeleted { get; set; } = false;
        public string? GameFilePath { get; set; }
        public ICollection<GameVersion> Versions { get; set; } = new List<GameVersion>();

    }
}
