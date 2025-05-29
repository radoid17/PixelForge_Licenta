using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelForge.Models;
using PixelForge.Areas.Identity.Data;

namespace PixelForge.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly UserDbContext _context;
        public GameController(UserDbContext context) {  
            _context = context; 
        }
        public async Task<IActionResult> Index()
        {
            var game = await _context.Games.ToListAsync();
            return View(game);
        }
        public async Task<IActionResult> Store()
        {
            var game = await _context.Games.ToListAsync();
            return View(game);
        }

        public async Task<IActionResult> Library()
        {
            var game = await _context.Games.ToListAsync();
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
                _context.Games.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == id);
            return View(game);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Price")] Game game)
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
