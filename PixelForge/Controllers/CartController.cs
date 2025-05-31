using Microsoft.AspNetCore.Mvc;
using PixelForge.Models;
using PixelForge.SessionExtensions;
using PixelForge.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PixelForge.Controllers
{
    public class CartController : Controller
    {
        private readonly UserDbContext _context;

        public CartController(UserDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (!cart.Any(c => c.GameId == id))
            {
                var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
                if (game != null)
                {
                    cart.Add(new CartItem
                    {
                        GameId = game.Id,
                        Title = game.Title,
                        Price = game.Price
                    });

                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart != null)
            {
                var itemToRemove = cart.FirstOrDefault(c => c.GameId == id);
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Challenge();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var ownedGameIds = await _context.UserGames
                .Where(ug => ug.UserId == userId)
                .Select(ug => ug.GameId)
                .ToListAsync();

            foreach (var item in cart)
            {
                if (!ownedGameIds.Contains(item.GameId))
                {
                    _context.UserGames.Add(new UserGame
                    {
                        UserId = userId,
                        GameId = item.GameId
                    });
                }
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Purchase successful! Your games are now in your library.";
            return RedirectToAction("Library", "Game");
        }

    }
}
