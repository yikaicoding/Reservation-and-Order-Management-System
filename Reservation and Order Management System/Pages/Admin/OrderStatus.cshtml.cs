using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Admin
{
    public class OrderStatusModel : PageModel
    {
        public string? OrderReference { get; set; }

        public string? ReservationReference { get; set; }

        public string? CurrentStatus { get; set; }

        public int ProgressPercentage { get; set; }

        public void OnGet()
        {
        }
    }
}