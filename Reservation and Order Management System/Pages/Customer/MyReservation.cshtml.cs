using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US11 - Customer reservation history
    public class MyReservationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public MyReservationModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Reservation> Upcoming { get; set; } = new List<Reservation>();

        public List<Reservation> Past { get; set; } = new List<Reservation>();

        //Filter for the past list: All, Completed, Cancelled
        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; } = "All";

        public int TotalVisits { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //Must be logged in
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToPage("/Customer/Login");
            }

            //Only this customer's reservations, newest first
            var reservations = await _context.Reservations
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReservationDate)
                .ThenByDescending(r => r.ReservationTime)
                .ToListAsync();

            var now = DateTime.Now;

            //Upcoming = date/time not passed yet, and not cancelled or completed
            Upcoming = reservations
                .Where(r => r.ReservationDate.Date.Add(r.ReservationTime) >= now &&
                            r.Status != "Cancelled" &&
                            r.Status != "Completed")
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToList();

            //Everything else = past
            Past = reservations.Except(Upcoming).ToList();

            //Count visits that were not cancelled
            TotalVisits = Past.Count(r => r.Status != "Cancelled");

            //Apply filter to past list
            if (Filter == "Completed")
            {
                Past = Past.Where(r => r.Status != "Cancelled").ToList();
            }
            else if (Filter == "Cancelled")
            {
                Past = Past.Where(r => r.Status == "Cancelled").ToList();
            }
            else
            {
                Filter = "All";
            }

            return Page();
        }

        //Bootstrap badge colour for each status
        public static string StatusBadge(string status)
        {
            switch (status)
            {
                case "Confirmed": return "bg-success";
                case "Reserved": return "bg-success";
                case "Pending": return "bg-warning text-dark";
                case "Completed": return "bg-secondary";
                case "Cancelled": return "bg-danger";
                default: return "bg-light text-dark";
            }
        }
    }
}