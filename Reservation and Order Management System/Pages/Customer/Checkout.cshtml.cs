using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Payments
{
    public class CheckoutModel : PageModel
    {
        [BindProperty]
        public string? CardholderName { get; set; }

        [BindProperty]
        public string? CardNumber { get; set; }

        [BindProperty]
        public string? ExpiryDate { get; set; }

        [BindProperty]
        public string? CVV { get; set; }

        public string? OrderReference { get; set; }

        public string? BookingReference { get; set; }

        public decimal? TotalAmount { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("PaymentSuccess");
        }
    }
}