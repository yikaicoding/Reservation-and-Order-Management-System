using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US01 - Customer Registration
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Customer Customer { get; set; } = new Models.Customer();

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

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
            //Check password confirmation
            if (Customer.Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
            }

            //Check password length
            if (!string.IsNullOrEmpty(Customer.Password) && Customer.Password.Length < 6)
            {
                ModelState.AddModelError("Customer.Password", "Password must be at least 6 characters.");
            }

            //Check whether email already exists
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == Customer.Email);

            if (existingCustomer != null)
            {
                ModelState.AddModelError("Customer.Email", "This email is already registered.");
            }

            //Check validation
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Add customer to database
            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            //Success Message
            TempData["SuccessMessage"] = "You can now login with your registered account.";

            //Go to Login page
            return RedirectToPage("/Customer/Login");
        }
    }
}