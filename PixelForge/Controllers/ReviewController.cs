using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelForge.Areas.Identity.Data;
using PixelForge.Models;

namespace PixelForge.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly UserDbContext _context;
        private readonly UserManager<PixelForgeUser> _userManager;

        public ReviewController(UserDbContext context, UserManager<PixelForgeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int gameId, int rating, string comment)
        {
            var user = await _userManager.GetUserAsync(User);

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.GameId == gameId && r.UserId == user.Id);

            if (alreadyReviewed)
                return BadRequest("You already reviewed this game.");

            var review = new Review
            {
                GameId = gameId,
                UserId = user.Id,
                Rating = rating,
                Comment = comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Library", "Game");
        }
    }
}
