using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US01 - Customer Dashboard (landing page after login)
    public class DashboardModel : PageModel
    {
        public string CustomerName { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            //Must be logged in
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return RedirectToPage("/Customer/Login");
            }

            CustomerName = HttpContext.Session.GetString("CustomerName") ?? "Customer";

            return Page();
        }
    }
}