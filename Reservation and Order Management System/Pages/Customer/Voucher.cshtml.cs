using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US07 - Printable reservation voucher
    public class VoucherModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public VoucherModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string BookingId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ReservationDate { get; set; } = string.Empty;
        public string ReservationTime { get; set; } = string.Empty;
        public int GuestCount { get; set; }
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

            BookingId = "QD-" + reservation.ReservationId.ToString("D5");
            CustomerName = HttpContext.Session.GetString("CustomerName") ?? "Customer";
            ReservationDate = reservation.ReservationDate.ToString("dddd, d MMM yyyy");
            ReservationTime = DateTime.Today.Add(reservation.ReservationTime).ToString("h:mm tt");
            GuestCount = reservation.NumberOfGuests;
            TableNumber = reservation.RestaurantTable?.TableNumber ?? reservation.RestaurantTableId.ToString();

            return Page();
        }
    }
}