using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.Models;
using Admin_Dashboard.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SanayiiContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSignalR();
// ...


// Configure Identity with your custom AppUser
builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()  // Add this if you're using roles
.AddEntityFrameworkStores<SanayiiContext>()
.AddDefaultTokenProviders()
.AddSignInManager<SignInManager<AppUser>>();  // Explicitly add SignInManager

// Add other services
builder.Services.AddRazorPages();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<UnitOFWork>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Configure token lifespan
// In Program.cs
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(24); // Set your desired expiration
});

// Email configuration
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add authentication and authorization services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.MapHub<DashboardHub>("/dashboardHub");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Test email sending (remove in production)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    try
    {
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        await emailSender.SendEmailAsync("test@example.com", "Test Email", "This is a test email");
        app.Logger.LogInformation("Test email sent successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error sending test email");
    }
}

app.Run();