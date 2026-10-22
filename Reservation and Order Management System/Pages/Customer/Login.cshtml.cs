using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US01 - Customer Login
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [TempData]
        public string? SuccessMessage { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            //Already logged in -> go to dashboard
            if (HttpContext.Session.GetInt32("CustomerId") != null)
            {
                return RedirectToPage("/Customer/Dashboard");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your email and password.";
                return Page();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == Email && c.Password == Password);

            if (customer == null)
            {
                ErrorMessage = "Invalid email or password";
                return Page();
            }

            //Store Customer ID and name in session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            HttpContext.Session.SetString("CustomerName", customer.FullName);

            //Go to Customer Dashboard
            return RedirectToPage("/Customer/Dashboard");
        }
    }
}