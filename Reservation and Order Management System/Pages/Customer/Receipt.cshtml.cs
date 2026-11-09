using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Payments
{
    public class ReceiptModel : PageModel
    {
        public string? ReceiptNumber { get; set; }

        public string? TransactionReference { get; set; }

        public string? BookingReference { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? PaymentMethod { get; set; }

        public decimal? Amount { get; set; }

        public void OnGet()
        {
        }
    }
}