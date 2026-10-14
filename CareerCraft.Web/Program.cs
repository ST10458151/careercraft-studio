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
    // This is a local-only MVP; a production database requires audited EF migrations.
    // Never silently create/modify a live customer database at application startup.
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException(
            "CareerCraft Studio is not ready for production hosting. Run locally with the http Development launch profile; production requires migrations and security hardening.");

    await database.Database.EnsureCreatedAsync();
    // EnsureCreated cannot extend an existing database that was created with a
    // different schema. Detect it explicitly rather than failing on login.
    var identityTables = await database.Database
        .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sqlite_master WHERE type='table' AND name IN ('AspNetUsers','AspNetRoles')")
        .SingleAsync();
    if (identityTables != 2)
        throw new InvalidOperationException(
            "The local careercraft.db has an incomplete/old schema (missing Identity tables). Back up the existing database and create a fresh local database, or migrate its data. Do not delete a database containing real data.");

    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roles.RoleExistsAsync("Admin"))
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
