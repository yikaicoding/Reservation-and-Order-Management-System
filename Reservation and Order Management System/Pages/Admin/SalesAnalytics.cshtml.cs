using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Admin
{
    public class SalesAnalyticsModel : PageModel
    {
        public decimal TotalRevenue { get; set; }

        public int TotalOrders { get; set; }

        public int TotalReservations { get; set; }

        public int TotalCustomers { get; set; }

        public int PreparingOrders { get; set; }

        public int ReadyOrders { get; set; }

        public int CompletedOrders { get; set; }

        public List<TopSellingItemDto> TopSellingItems { get; set; }
            = new();

        public void OnGet()
        {
        }
    }

    public class TopSellingItemDto
    {
        public string ItemName { get; set; } = string.Empty;

        public int QuantitySold { get; set; }
    }
}