using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    public class ReserveTableModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReserveTableModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public ReservationInputModel ReservationInput { get; set; } = new();

        public SelectList TableOptions { get; set; } = default!;

        public class ReservationInputModel
        {
            public int RestaurantTableId { get; set; }
            public DateTime ReservationDate { get; set; } = DateTime.Today.AddDays(1);
            public string TimeSlot { get; set; } = "18:00 - 20:00";
            public int NumberOfGuests { get; set; } = 2;
            public string? SpecialRequests { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAvailableTablesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableTablesAsync();
                return Page();
            }

            // Retrieve selected table
            var table = await _context.RestaurantTables.FindAsync(ReservationInput.RestaurantTableId);
            if (table == null)
            {
                ModelState.AddModelError(string.Empty, "Selected table is no longer available.");
                await LoadAvailableTablesAsync();
                return Page();
            }

            // Validate capacity
            if (ReservationInput.NumberOfGuests > table.Capacity)
            {
                ModelState.AddModelError("ReservationInput.NumberOfGuests",
                    $"Table {table.TableNumber} seats a maximum of {table.Capacity} guests.");
                await LoadAvailableTablesAsync();
                return Page();
            }

            TempData["SuccessMessage"] = $"Table {table.TableNumber} reserved successfully for {ReservationInput.ReservationDate:dd MMM yyyy} ({ReservationInput.TimeSlot})!";
            return RedirectToPage("/Customer/ReserveTable");
        }

        private async Task LoadAvailableTablesAsync()
        {
            var tables = await _context.RestaurantTables
                .Where(t => t.Status == "Available")
                .OrderBy(t => t.DiningArea)
                .ThenBy(t => t.TableNumber)
                .Select(t => new
                {
                    t.RestaurantTableId,
                    DisplayText = $"{t.TableNumber} - {t.DiningArea} (Seats {t.Capacity})"
                })
                .ToListAsync();

            TableOptions = new SelectList(tables, "RestaurantTableId", "DisplayText");
        }
    }
}