using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Payments
{
    public class PaymentSuccessModel : PageModel
    {
        public string? TransactionReference { get; set; }

        public void OnGet()
        {
        }
    }
}