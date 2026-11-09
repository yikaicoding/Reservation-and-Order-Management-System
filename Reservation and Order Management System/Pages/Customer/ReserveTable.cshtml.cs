using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    public class ReserveTableModel : PageModel
    {
        public void OnGet()
        {
            // Step 1: Schedule & Discovery Landing Page.
            // Search inputs (date, timeSlot, guests) post directly via GET to /Customer/SelectTable.
        }
    }
}