using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelForge.Areas.Identity.Data;
using PixelForge.Models;
using Microsoft.AspNetCore.Identity;

namespace PixelForge.Controllers
{
    [Authorize]
    public class SupportController : Controller
    {
        private readonly UserDbContext _context;
        private readonly UserManager<PixelForgeUser> _userManager;

        public SupportController(UserDbContext context, UserManager<PixelForgeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.Publisher)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            List<SupportMessage> messages;

            if (User.IsInRole("Publisher") && game.PublisherId == currentUserId)
            {
                messages = await _context.SupportMessages
                    .Include(m => m.Author)
                    .Where(m => m.GameId == gameId)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();
            }
            else if (User.IsInRole("User"))
            {
                messages = await _context.SupportMessages
                    .Include(m => m.Author)
                    .Where(m => m.GameId == gameId && m.AuthorId == currentUserId)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                return Unauthorized();
            }

            ViewBag.Game = game;
            return View(messages);
        }

        public async Task<IActionResult> Send(int gameId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null)
                return NotFound();

            ViewBag.Game = game;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(SupportMessage message)
        {
            var userId = _userManager.GetUserId(User);
            var game = await _context.Games.FindAsync(message.GameId);

            if (game == null)
                return NotFound();

            message.AuthorId = userId;
            message.CreatedAt = DateTime.Now;
            _context.SupportMessages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Game", new { id = message.GameId });
        }

        [Authorize(Roles = "Publisher")]
        public async Task<IActionResult> Reply(int id)
        {
            var message = await _context.SupportMessages
                .Include(m => m.Game)
                .Include(m => m.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            var currentUserId = _userManager.GetUserId(User);

            if (message == null || message.Game.PublisherId != currentUserId)
                return Unauthorized();

            return View(message);
        }

        [HttpPost]
        [Authorize(Roles = "Publisher")]
        public async Task<IActionResult> Reply(int id, string reply)
        {
            var message = await _context.SupportMessages
                .Include(m => m.Game)
                .Include(m => m.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            var currentUserId = _userManager.GetUserId(User);

            if (message == null || message.Game.PublisherId != currentUserId)
                return Unauthorized();

            message.PublisherReply = reply;
            message.ReplyDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { gameId = message.GameId });
        }


    }
}
