using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //Customer home page after login
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string CustomerName { get; set; } = string.Empty;

        public string Greeting { get; set; } = string.Empty;

        //Nearest upcoming reservation (null if none)
        public Reservation? NextReservation { get; set; }

        //Most recent past visit that has not been reviewed (null if none)
        public Reservation? ToReview { get; set; }

        //Last 3 reservations
        public List<Reservation> Recent { get; set; } = new List<Reservation>();

        //Stats
        public int TotalVisits { get; set; }
        public int UpcomingCount { get; set; }
        public int ReviewsGiven { get; set; }

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

            //All of this customer's reservations
            var reservations = await _context.Reservations
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReservationDate)
                .ThenByDescending(r => r.ReservationTime)
                .ToListAsync();

            var now = DateTime.Now;

            //Upcoming = not passed yet, not cancelled or completed
            var upcoming = reservations
                .Where(r => r.ReservationDate.Date.Add(r.ReservationTime) >= now &&
                            r.Status != "Cancelled" &&
                            r.Status != "Completed")
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToList();

            //Past visits that actually happened
            var pastVisits = reservations
                .Where(r => r.ReservationDate.Date.Add(r.ReservationTime) < now &&
                            r.Status != "Cancelled")
                .ToList();

            NextReservation = upcoming.FirstOrDefault();
            UpcomingCount = upcoming.Count;
            TotalVisits = pastVisits.Count;
            Recent = reservations.Take(3).ToList();

            //Which visits already have a review
            var reviewedIds = await _context.Reviews
                .Where(rv => rv.Reservation!.CustomerId == customerId)
                .Select(rv => rv.ReservationId)
                .ToListAsync();

            ReviewsGiven = reviewedIds.Count;

            //Latest past visit without a review
            ToReview = pastVisits
                .FirstOrDefault(r => !reviewedIds.Contains(r.ReservationId));

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