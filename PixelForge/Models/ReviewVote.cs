using PixelForge.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace PixelForge.Models
{
    public class ReviewVote
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ReviewId { get; set; }
        public bool IsLike { get; set; }
        public PixelForgeUser User { get; set; }
        public Review Review { get; set; }
    }
}
