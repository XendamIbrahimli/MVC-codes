using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniqloMVC.DataAccess;
using UniqloMVC.Extentions;
using UniqloMVC.Models;

namespace UniqloMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.ConfigureApplicationCookie(x =>
            {
                x.AccessDeniedPath = "/Home/AccessDenied";
            });

            builder.Services.AddDbContext<UniqloDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MsSql"));
            });

            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 3;
                opt.Password.RequireNonAlphanumeric=false;
                opt.Password.RequireDigit= false;
                opt.Password.RequireUppercase= false;
                opt.Password.RequireLowercase= false;
                opt.Lockout.MaxFailedAccessAttempts = 2;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<UniqloDbContext>();

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseUserSeed();  

            app.MapControllerRoute(name: "register",
                pattern: "register",
                defaults: new { controller = "Account", action = "Register" });

            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{Id?}");

            app.Run();
        }
    }
}
