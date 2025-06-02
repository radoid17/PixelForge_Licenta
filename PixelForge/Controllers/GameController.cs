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
                .Where(g => !g.IsDeleted)
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
                .Include(g => g.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            return View(game);
        }


        public IActionResult Create() { 
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Title, Price")] Game game, IFormFile gameFile)
        {
            var allowedExtensions = new[] { ".zip", ".rar", ".exe", ".msi" };
            var errors = new List<string>();

            if (gameFile == null || gameFile.Length == 0)
            {
                errors.Add("Please upload a game file.");
            }
            else
            {
                var extension = Path.GetExtension(gameFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    errors.Add("Only .zip, .rar, .exe, and .msi files are allowed.");
                }
            }

            if (!ModelState.IsValid || errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View(game);
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(gameFile.FileName)}";
            var fullPath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await gameFile.CopyToAsync(stream);
            }

            game.GameFilePath = $"/uploads/{uniqueFileName}";

            game.PublisherId = _userManager.GetUserId(User);
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
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


        public async Task<IActionResult> Download(int id)
        {
            var userId = _userManager.GetUserId(User);
            var hasGame = await _context.UserGames.AnyAsync(ug => ug.UserId == userId && ug.GameId == id);

            if (!hasGame)
                return Unauthorized();

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);

            if (game == null || string.IsNullOrEmpty(game.GameFilePath))
                return NotFound();

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", game.GameFilePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found on server.");

            var fileName = Path.GetFileName(fullPath);
            var contentType = "application/octet-stream";

            var memory = new MemoryStream();
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, contentType, fileName);
        }

    }
}
