using Microsoft.AspNetCore.Identity;
using ProniaWebApp.Services;

namespace ProniaWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            //builder.Services.AddSession(opt=>
            //{
            //    opt.IdleTimeout=TimeSpan.FromSeconds(5);
            //});

            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 10;
                opt.Password.RequireNonAlphanumeric = true;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();



            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddScoped<LayoutService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var app = builder.Build();
			//app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

			app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            app.UseStaticFiles();

            app.Run();
        }
    }
}