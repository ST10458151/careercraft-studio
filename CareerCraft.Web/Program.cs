using CareerCraft.Web.Data;
using CareerCraft.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.Configure<StudioSettings>(builder.Configuration.GetSection("Studio"));
builder.Services.AddSingleton<ServiceCatalogue>();
builder.Services.AddDbContext<StudioDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Studio") ?? "Data Source=careercraft.db"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Local development only; add verified email before launch.
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<StudioDbContext>()
.AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Denied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<StudioDbContext>();
    // Local-development starter only: replace EnsureCreated with EF migrations before production.
    if (app.Environment.IsDevelopment()) database.Database.EnsureCreated();
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (database.Database.CanConnect() && !await roles.RoleExistsAsync("Admin"))
        await roles.CreateAsync(new IdentityRole("Admin"));
    // Explicit local-only admin bootstrap; never expose admin registration publicly.
    if (app.Environment.IsDevelopment() && args.Contains("--seed-admin"))
    {
        var email = Environment.GetEnvironmentVariable("CAREERCRAFT_ADMIN_EMAIL");
        var password = Environment.GetEnvironmentVariable("CAREERCRAFT_ADMIN_PASSWORD");
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Set CAREERCRAFT_ADMIN_EMAIL and CAREERCRAFT_ADMIN_PASSWORD before --seed-admin.");
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var admin = await users.FindByEmailAsync(email);
        if (admin == null)
        {
            admin = new IdentityUser { UserName=email, Email=email, EmailConfirmed=true };
            var result = await users.CreateAsync(admin, password);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ",result.Errors.Select(x=>x.Description)));
        }
        if (!await users.IsInRoleAsync(admin,"Admin")) await users.AddToRoleAsync(admin,"Admin");
        Console.WriteLine("Local admin account is provisioned. Remove seed credentials from your shell environment.");
    }
}
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default","{controller=Home}/{action=Index}/{id?}");
app.Run();
