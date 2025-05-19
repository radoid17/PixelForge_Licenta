using Microsoft.EntityFrameworkCore;
using PixelForge.Models;

namespace PixelForge.Controllers.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
        
    }
}
