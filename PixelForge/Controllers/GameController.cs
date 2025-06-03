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
        public async Task<IActionResult> Store(string searchString, string sortOrder, string genreFilter, double? minPrice, double? maxPrice, string ageFilter)
        {
            var game = await _context.Games
                .Include(g => g.Publisher)
                .Where(g => !g.IsDeleted)
                .ToListAsync();

            // 🔍 Filtrare după titlu
            if (!string.IsNullOrEmpty(searchString))
            {
                game = game.Where(g => g.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 🔍 Filtrare după gen
            if (!string.IsNullOrEmpty(genreFilter))
            {
                game = game.Where(g => g.Genre.Equals(genreFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 🔍 Filtrare după preț
            if (minPrice.HasValue)
            {
                game = game.Where(g => g.Price >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                game = game.Where(g => g.Price <= maxPrice.Value).ToList();
            }

            // 🔍 Filtrare după rating vârstă
            if (!string.IsNullOrEmpty(ageFilter))
            {
                game = game.Where(g => g.AgeRating.ToString() == ageFilter).ToList();
            }


            ViewData["NameSortParam"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["PriceSortParam"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["PublisherSortParam"] = sortOrder == "publisher_asc" ? "publisher_desc" : "publisher_asc";
            ViewData["GenreSortParam"] = sortOrder == "genre_asc" ? "genre_desc" : "genre_asc";
            ViewData["AgeSortParam"] = sortOrder == "age_asc" ? "age_desc" : "age_asc";
            ViewData["PopularitySortParam"] = sortOrder == "popularity_asc" ? "popularity_desc" : "popularity_asc";



            var gameWithCounts = game.Select(g => new
            {
                Game = g,
                OwnersCount = _context.UserGames.Count(ug => ug.GameId == g.Id)
            }).ToList();

            switch (sortOrder)
            {
                case "name_desc":
                    gameWithCounts = gameWithCounts.OrderByDescending(g => g.Game.Title).ToList();
                    break;
                case "name_asc":
                    gameWithCounts = gameWithCounts.OrderBy(g => g.Game.Title).ToList();
                    break;
                case "price_desc":
                    gameWithCounts = gameWithCounts.OrderByDescending(g => g.Game.Price).ToList();
                    break;
                case "price_asc":
                    gameWithCounts = gameWithCounts.OrderBy(g => g.Game.Price).ToList();
                    break;
                case "publisher_desc":
                    gameWithCounts = gameWithCounts.OrderByDescending(g => g.Game.Publisher.FirstName + " " + g.Game.Publisher.SecondName).ToList();
                    break;
                case "publisher_asc":
                    gameWithCounts = gameWithCounts.OrderBy(g => g.Game.Publisher.FirstName + " " + g.Game.Publisher.SecondName).ToList();
                    break;
                case "genre_desc":
                    gameWithCounts = gameWithCounts.OrderByDescending(g => g.Game.Genre).ToList();
                    break;
                case "genre_asc":
                    gameWithCounts = gameWithCounts.OrderBy(g => g.Game.Genre).ToList();
                    break;
                case "age_desc":
                    gameWithCounts = gameWithCounts.OrderByDescending(g => g.Game.AgeRating).ToList();
                    break;
                case "age_asc":
                    gameWithCounts = gameWithCounts.OrderBy(g => g.Game.AgeRating).ToList();
                    break;
                case "popularity_desc":
                    gameWithCounts = gameWithCounts.OrderByDescending(g => g.OwnersCount).ToList();
                    break;
                case "popularity_asc":
                    gameWithCounts = gameWithCounts.OrderBy(g => g.OwnersCount).ToList();
                    break;
                default:
                    gameWithCounts = gameWithCounts.OrderBy(g => g.Game.Title).ToList();
                    break;
            }

            game = gameWithCounts.Select(g => g.Game).ToList();


            return View(game);
        }

        public async Task<IActionResult> Library(string searchString, string sortOrder, string genreFilter, string ageFilter)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Challenge();

            var ownedGames = await _context.UserGames
                .Where(ug => ug.UserId == userId)
                .Include(ug => ug.Game)
                .Select(ug => ug.Game)
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                ownedGames = ownedGames.Where(g => g.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 🔍 Filtrare după gen
            if (!string.IsNullOrEmpty(genreFilter))
            {
                ownedGames = ownedGames.Where(g => g.Genre.Equals(genreFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 🔍 Filtrare după rating vârstă
            if (!string.IsNullOrEmpty(ageFilter))
            {
                ownedGames = ownedGames.Where(g => g.AgeRating.ToString() == ageFilter).ToList();
            }

            ViewData["NameSortParam"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["GenreSortParam"] = sortOrder == "genre_asc" ? "genre_desc" : "genre_asc";
            ViewData["AgeSortParam"] = sortOrder == "age_asc" ? "age_desc" : "age_asc";


            switch (sortOrder)
            {
                case "name_desc":
                    ownedGames = ownedGames.OrderByDescending(g => g.Title).ToList();
                    break;
                case "name_asc":
                    ownedGames = ownedGames.OrderBy(g => g.Title).ToList();
                    break;

                case "genre_desc":
                    ownedGames = ownedGames.OrderByDescending(g => g.Genre).ToList();
                    break;
                case "genre_asc":
                    ownedGames = ownedGames.OrderBy(g => g.Genre).ToList();
                    break;

                case "age_desc":
                    ownedGames = ownedGames.OrderByDescending(g => g.AgeRating).ToList();
                    break;
                case "age_asc":
                    ownedGames = ownedGames.OrderBy(g => g.AgeRating).ToList();
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

            var ownersCount = await _context.UserGames.CountAsync(ug => ug.GameId == game.Id);
            ViewBag.OwnersCount = ownersCount;

            if (game == null)
                return NotFound();

            return View(game);
        }


        public IActionResult Create() { 
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Title, Price, Genre, AgeRating")] Game game, IFormFile gameFile)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Price, PublisherId, Genre, AgeRating, GameFilePath")] Game game)
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
