using PixelForge.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace PixelForge.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public PixelForgeUser User { get; set; }

        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
