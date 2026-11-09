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
            //Must be logged in (US01 session)
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return RedirectToPage("/Customer/Login");
            }

            await LoadAvailableTablesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Must be logged in (US01 session)
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return RedirectToPage("/Customer/Login");
            }

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

            // Start time from the time slot, e.g. "18:00 - 20:00" -> 18:00
            var startText = ReservationInput.TimeSlot.Split('-')[0].Trim();
            if (!TimeSpan.TryParse(startText, out var startTime))
            {
                startTime = new TimeSpan(18, 0, 0);
            }

            var start = ReservationInput.ReservationDate.Date.Add(startTime);
            var end = start.AddHours(2);

            // No bookings in the past
            if (start <= DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "Please choose a date and time in the future.");
                await LoadAvailableTablesAsync();
                return Page();
            }

            // Double-booking check (US05): same table, overlapping time, not cancelled
            bool overlaps = await _context.Reservations.AnyAsync(r =>
                r.RestaurantTableId == table.RestaurantTableId &&
                r.Status != "Cancelled" &&
                r.StartsAt < end &&
                r.EndsAt > start);

            if (overlaps)
            {
                ModelState.AddModelError(string.Empty,
                    $"Table {table.TableNumber} is already booked for that time. Please choose another table or time slot.");
                await LoadAvailableTablesAsync();
                return Page();
            }

            // Go to step 3 - Dining Preferences (US06)
            return RedirectToPage("/Customer/DiningPreferences", new
            {
                date = start.ToString("yyyy-MM-dd"),
                time = startTime.ToString(@"hh\:mm"),
                guests = ReservationInput.NumberOfGuests,
                tableId = table.RestaurantTableId
            });
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