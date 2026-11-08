using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reservation_and_Order_Management_System.Data;
using RestaurantReservationSystem.Models;

namespace Reservation_and_Order_Management_System.Pages.Account;

[AllowAnonymous]
public class RegisterModel(
    UserManager<IdentityUser> users,
    SignInManager<IdentityUser> signIn,
    ApplicationDbContext db) : PageModel
{
    [BindProperty, Required, StringLength(100)]
    public string FullName { get; set; } = "";

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = "";

    [BindProperty, Required, MinLength(8), DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [BindProperty, Required, Compare(nameof(Password)), DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var email = Email.Trim();

        var user = new IdentityUser
        {
            UserName = email,
            Email = email
        };

        var result = await users.CreateAsync(user, Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        db.CustomerProfiles.Add(new CustomerProfile
        {
            CustomerId = user.Id,
            FullName = FullName.Trim(),
            Email = email
        });

        await db.SaveChangesAsync();
        await signIn.SignInAsync(user, isPersistent: false);

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        return RedirectToPage("/Reservations/Create");
    }
}