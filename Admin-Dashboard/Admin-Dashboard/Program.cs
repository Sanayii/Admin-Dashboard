using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.Models;
using Microsoft.Extensions.Options;
using System.Configuration;
using Admin_Dashboard.Services;

namespace Admin_Dashboard;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<SanayiiContext>(options =>
            options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddDefaultIdentity<AppUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false; 
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<SanayiiContext>();

        // Add Razor Pages
        builder.Services.AddRazorPages(); // Add this line to register Razor Pages services

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddScoped<UnitOFWork>(); // Ensure UnitOfWork is added here
        builder.Services.AddScoped<INotificationService,NotificationService>();
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient();

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
        app.UseAuthentication();
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
