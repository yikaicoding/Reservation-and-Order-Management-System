using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Account;

[AllowAnonymous]
public class LoginModel(SignInManager<IdentityUser> signIn) : PageModel
{
    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = "";

    [BindProperty, Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await signIn.PasswordSignInAsync(
            Email,
            Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Email or password is incorrect.");
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        //Send each role to its own area
        var user = await signIn.UserManager.FindByEmailAsync(Email);
        if (user != null && await signIn.UserManager.IsInRoleAsync(user, "Admin"))
            return RedirectToPage("/Admin/ManageTables");

        if (user != null && await signIn.UserManager.IsInRoleAsync(user, "Staff"))
            return RedirectToPage("/Staff/Reservations");

        return RedirectToPage("/Customer/Dashboard");
    }
}
