using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PixelForge.Areas.Identity.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PixelForge
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();


            builder.Services.AddDbContext<UserDbContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

            builder.Services.AddDefaultIdentity<PixelForgeUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<UserDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] {"Publisher", "User"};

                foreach (var role in roles) {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<PixelForgeUser>>();

                string FirstName = "Publisherescu";
                string SecondName = "Ioan";
                string email = "publisher@gmail.com";
                string password = "8TC)-70h02#o";

                if (await userManager.FindByEmailAsync(email) == null) {
                    var user = new PixelForgeUser();
                    user.FirstName = FirstName;
                    user.SecondName = SecondName;
                    user.UserName = email;
                    user.Email = email;
                    user.EmailConfirmed = true;

                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Publisher");
                }
            }

            app.Run();
        }
    }
}
