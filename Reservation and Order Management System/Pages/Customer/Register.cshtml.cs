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

        [BindProperty]
        public string CountryCode { get; set; } = "+60";

        [BindProperty]
        public bool AgreeTerms { get; set; }

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
            //Check password rules: 8+ characters with at least one number
            if (!string.IsNullOrEmpty(Customer.Password) &&
                (Customer.Password.Length < 8 || !Customer.Password.Any(char.IsDigit)))
            {
                ModelState.AddModelError("Customer.Password",
                    "Use at least 8 characters, including a number.");
            }

            //Check password confirmation
            if (Customer.Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
            }

            //Check terms accepted
            if (!AgreeTerms)
            {
                ModelState.AddModelError("AgreeTerms",
                    "Please agree to the Terms of Service and Privacy Policy.");
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

            //Save phone with country code, e.g. "+60 12-345 6789"
            Customer.Phone = CountryCode + " " + Customer.Phone.Trim();
            Customer.FullName = Customer.FullName.Trim();
            Customer.Email = Customer.Email.Trim();

            //Add customer to database
            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            //Success Message
            TempData["SuccessMessage"] = "Your account is ready. Sign in to continue.";

            //Go to Login page
            return RedirectToPage("/Customer/Login");
        }
    }
}