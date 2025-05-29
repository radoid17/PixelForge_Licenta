using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelForge.Models;
using PixelForge.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace PixelForge.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly UserDbContext _context;
        private readonly UserManager<PixelForgeUser> _userManager;

        public GameController(UserDbContext context, UserManager<PixelForgeUser> userManager) {  
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var game = await _context.Games
                .Where(g => g.PublisherId == userId)
                .ToListAsync();
            return View(game);
        }
        public async Task<IActionResult> Store()
        {
            var game = await _context.Games
                .Include(g => g.Publisher)
                .ToListAsync();

            return View(game);
        }

        public async Task<IActionResult> Library()
        {
            var game = await _context.Games.ToListAsync();
            return View(game);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var game = await _context.Games
                .Include(g => g.Publisher)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            return View(game);
        }


        public IActionResult Create() { 
            return View();
        }
        [HttpPost] 
        public async Task<IActionResult> Create([Bind("Id, Title, Price")] Game game)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                game.PublisherId = userId;

                _context.Games.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == id);
            var userId = _userManager.GetUserId(User);

            if (game == null || game.PublisherId != userId)
                return Unauthorized();

            return View(game);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Price, PublisherId")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Games.Update(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == id);
            var userId = _userManager.GetUserId(User);

            if (game == null || game.PublisherId != userId)
                return Unauthorized();

            return View(game);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null) { 
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
