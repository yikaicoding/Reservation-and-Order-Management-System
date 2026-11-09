using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    public class SelectTableModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SelectTableModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TimeSlot { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int Guests { get; set; }

        [BindProperty]
        public int SelectedTableId { get; set; }

        public IList<RestaurantTable> Tables { get; set; } = new List<RestaurantTable>();
        public List<int> ReservedTableIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (Date == DateTime.MinValue) Date = DateTime.Today;

            Tables = await _context.RestaurantTables.ToListAsync();

            ReservedTableIds = await _context.Reservations
                .Where(r => r.ReservationDate.Date == Date.Date && r.TimeSlot == TimeSlot && r.Status != "Cancelled")
                .Select(r => r.RestaurantTableId)
                .ToListAsync();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (SelectedTableId <= 0)
            {
                ModelState.AddModelError("", "Please select a table before continuing.");
                return Page();
            }

            return RedirectToPage("/Customer/Preferences", new
            {
                tableId = SelectedTableId,
                date = Date.ToString("yyyy-MM-dd"),
                timeSlot = TimeSlot,
                guests = Guests
            });
        }
    }
}