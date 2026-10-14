using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Pages.Admin
{
    public class ManageTablesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageTablesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<RestaurantTable> Tables { get; set; } = new List<RestaurantTable>();

        [BindProperty]
        public RestaurantTable NewTable { get; set; } = new();

        public async Task OnGetAsync()
        {
            Tables = await _context.RestaurantTables
                .OrderBy(t => t.DiningArea)
                .ThenBy(t => t.TableNumber)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Tables = await _context.RestaurantTables.ToListAsync();
                return Page();
            }

            // Check if TableNumber already exists
            bool exists = await _context.RestaurantTables
                .AnyAsync(t => t.TableNumber == NewTable.TableNumber);

            if (exists)
            {
                ModelState.AddModelError("NewTable.TableNumber", "A table with this number already exists.");
                Tables = await _context.RestaurantTables.ToListAsync();
                return Page();
            }

            _context.RestaurantTables.Add(NewTable);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}