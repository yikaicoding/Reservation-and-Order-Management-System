using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSession();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Seed default roles and admin account on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await Reservation_and_Order_Management_System.Data.DbInitializer.SeedRolesAndAdminAsync(services);
    await DbSeeder.SeedTablesAsync(services);
}

app.Run();
