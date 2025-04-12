using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Data;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.Models;

namespace Admin_Dashboard;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<SanayiiContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register Identity services correctly
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders(); // Adds RoleManager<IdentityRole> and UserManager<IdentityUser>

        // Add Razor Pages
        builder.Services.AddRazorPages(); // Add this line to register Razor Pages services

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddScoped<UnitOFWork>(); // Ensure UnitOfWork is added here
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
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

        app.MapRazorPages(); // Add this line to map Razor Pages endpoints

        app.Run();
    }
}
