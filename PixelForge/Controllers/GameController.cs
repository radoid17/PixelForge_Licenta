using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelForge.Models;
using PixelForge.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using PixelForge.Migrations;

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
        public async Task<IActionResult> Store(string searchString, string sortOrder)
        {
            var game = await _context.Games
                .Include(g => g.Publisher)
                .Where(g => !g.IsDeleted)
                .ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                game = game.Where(game => game.Title.Contains(searchString)).ToList();
            }

            ViewData["NameSortParam"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["PriceSortParam"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["PublisherSortParam"] = sortOrder == "publisher_asc" ? "publisher_desc" : "publisher_asc";

            switch (sortOrder)
            {
                case "name_desc":
                    game = game.OrderByDescending(g => g.Title).ToList();
                    break;
                case "name_asc":
                    game = game.OrderBy(g => g.Title).ToList();
                    break;

                case "price_desc":
                    game = game.OrderByDescending(g => g.Price).ToList();
                    break;
                case "price_asc":
                    game = game.OrderBy(g => g.Price).ToList();
                    break;

                case "publisher_desc":
                    game = game.OrderByDescending(g => g.Publisher.FirstName + " " + g.Publisher.SecondName).ToList();
                    break;
                case "publisher_asc":
                    game = game.OrderBy(g => g.Publisher.FirstName + " " + g.Publisher.SecondName).ToList();
                    break;

                default:
                    game = game.OrderBy(g => g.Title).ToList();
                    break;
            }

            return View(game);
        }

        public async Task<IActionResult> Library(string searchString, string sortOrder)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Challenge();

            var ownedGames = await _context.UserGames
                .Where(ug => ug.UserId == userId)
                .Include(ug => ug.Game)
                .Select(ug => ug.Game)
                .ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                ownedGames = ownedGames.Where(g => g.Title.Contains(searchString)).ToList();
            }

            ViewData["NameSortParam"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";

            switch (sortOrder)
            {
                case "name_desc":
                    ownedGames = ownedGames.OrderByDescending(g => g.Title).ToList();
                    break;
                case "name_asc":
                    ownedGames = ownedGames.OrderBy(g => g.Title).ToList();
                    break;

                default:
                    ownedGames = ownedGames.OrderBy(g => g.Title).ToList();
                    break;
            }

            return View(ownedGames);
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
                game.IsDeleted = true;
                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
