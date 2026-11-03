using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantReservationSystem.Data;
using RestaurantReservationSystem.Models;
using RestaurantReservationSystem.Models.ProfileViewModel;

namespace Reservation_and_Order_Management_System.Pages;

[Authorize]
public class ProfileModel(ApplicationDbContext db) : PageModel
{
    [BindProperty]
    public ProfileViewModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(
        CancellationToken cancellationToken)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Challenge();
        }

        var profile = await db.CustomerProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.CustomerId == customerId,
                cancellationToken);

        if (profile is not null)
        {
            Input = new ProfileViewModel
            {
                FullName = profile.FullName,
                Email = profile.Email,
                Phone = profile.PhoneNumber,
                DietaryPreferences = profile.DietaryPreferences,
                AccessibilityNotes = profile.DiningPreferences
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        CancellationToken cancellationToken)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Challenge();
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(
                string.Empty,
                "Your signed-in account must have an email address.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var profile = await db.CustomerProfiles
            .SingleOrDefaultAsync(
                item => item.CustomerId == customerId,
                cancellationToken);

        if (profile is null)
        {
            profile = new CustomerProfile
            {
                CustomerId = customerId,
                Email = email!
            };

            db.CustomerProfiles.Add(profile);
        }

        profile.FullName = Input.FullName.Trim();
        profile.Email = email!;
        profile.PhoneNumber = Input.Phone?.Trim() ?? "";
        profile.DietaryPreferences = Input.DietaryPreferences?.Trim();
        profile.DiningPreferences = Input.AccessibilityNotes?.Trim();

        await db.SaveChangesAsync(cancellationToken);

        TempData["Message"] = "Profile saved.";
        return RedirectToPage();
    }
}