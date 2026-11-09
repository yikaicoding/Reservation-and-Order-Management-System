using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Register Razor Pages & Session
builder.Services.AddRazorPages();
builder.Services.AddSession();

// 2. Register Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register Identity with Roles Support
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Authentication MUST be placed before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Seed default roles and admin account on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedRolesAndAdminAsync(services);
}

app.Run();