using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US07 - Booking confirmation (step 4)
    public class ConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ConfirmationModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string BookingId { get; set; } = string.Empty;
        public string ReservationDate { get; set; } = string.Empty;
        public string ReservationTime { get; set; } = string.Empty;
        public string TableNumber { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            //Must be logged in
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToPage("/Customer/Login");
            }

            //Only this customer's reservation
            var reservation = await _context.Reservations
                .Include(r => r.RestaurantTable)
                .FirstOrDefaultAsync(r => r.ReservationId == id && r.CustomerId == customerId);

            if (reservation == null)
            {
                return NotFound();
            }

            //Step 4: confirm the pending booking
            if (reservation.Status == "Pending")
            {
                reservation.Status = "Confirmed";
                await _context.SaveChangesAsync();
            }

            BookingId = "QD-" + reservation.ReservationId.ToString("D5");
            ReservationDate = reservation.ReservationDate.ToString("dddd, d MMM yyyy");
            ReservationTime = DateTime.Today.Add(reservation.ReservationTime).ToString("h:mm tt");
            TableNumber = reservation.RestaurantTable?.TableNumber ?? reservation.RestaurantTableId.ToString();

            return Page();
        }
    }
}