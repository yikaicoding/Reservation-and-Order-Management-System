using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Reservation_and_Order_Management_System.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "Staff", "Customer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed default Admin user for local testing
            var adminEmail = "admin@rroms.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            // Existing profile-backed accounts are customer accounts too.
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var customerIds = await db.CustomerProfiles
                .Select(profile => profile.CustomerId)
                .ToListAsync();

            foreach (var customerId in customerIds)
            {
                var customer = await userManager.FindByIdAsync(customerId);
                if (customer is not null && !await userManager.IsInRoleAsync(customer, "Customer"))
                {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }
            }
        }
    }
}


