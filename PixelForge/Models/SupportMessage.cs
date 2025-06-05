using PixelForge.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace PixelForge.Models
{
    public class SupportMessage
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public string AuthorId { get; set; }
        public PixelForgeUser Author { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? PublisherReply { get; set; }
        public DateTime? ReplyDate { get; set; }

    }
}
