using System.ComponentModel.DataAnnotations;

namespace PixelForge.Models
{
    public class GameVersion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Required]
        public string VersionNumber { get; set; } = "1.0";
        [Required]
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
