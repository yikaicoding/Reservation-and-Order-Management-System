using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //Customer home page after login (placeholder)
    public class DashboardModel : PageModel
    {
        public string CustomerName { get; set; } = string.Empty;

        public string Greeting { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            //Must be logged in
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return RedirectToPage("/Customer/Login");
            }

            CustomerName = HttpContext.Session.GetString("CustomerName") ?? "Customer";

            //Greeting based on time of day
            int hour = DateTime.Now.Hour;
            if (hour < 12)
            {
                Greeting = "Good morning";
            }
            else if (hour < 18)
            {
                Greeting = "Good afternoon";
            }
            else
            {
                Greeting = "Good evening";
            }

            return Page();
        }
    }
}