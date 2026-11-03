using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US01 - Customer Logout
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            //Remove everything stored in session
            HttpContext.Session.Clear();

            return RedirectToPage("/Customer/Login");
        }
    }
}