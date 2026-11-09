using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    public class PreferencesModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int TableId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TimeSlot { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int Guests { get; set; }

        [BindProperty]
        public string SpecialRequests { get; set; } = string.Empty;

        [BindProperty]
        public string DietaryRequirements { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (TableId <= 0)
            {
                return RedirectToPage("/Customer/ReserveTable");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            // Pass all parameters to Step 4 for final database insertion and voucher generation
            return RedirectToPage("/Customer/Confirmation", new
            {
                tableId = TableId,
                date = Date.ToString("yyyy-MM-dd"),
                timeSlot = TimeSlot,
                guests = Guests,
                specialRequests = $"{DietaryRequirements} | {SpecialRequests}".Trim(' ', '|')
            });
        }
    }
}