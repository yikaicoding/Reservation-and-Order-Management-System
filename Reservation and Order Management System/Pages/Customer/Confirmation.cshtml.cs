using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models; 

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    public class ConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ConfirmationModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Reservation CurrentReservation { get; set; } = null!;
        public RestaurantTable ReservedTable { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int tableId, DateTime date, string timeSlot, int guests, string specialRequests)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "GuestUser";

            var reservation = new Reservation
            {
                UserId = userId,
                RestaurantTableId = tableId,
                ReservationDate = date,
                TimeSlot = timeSlot,
                NumberOfGuests = guests,
                SpecialRequests = specialRequests,
                Status = "Confirmed"
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            CurrentReservation = reservation;
            ReservedTable = await _context.RestaurantTables.FirstOrDefaultAsync(t => t.RestaurantTableId == tableId)
                            ?? new RestaurantTable { TableNumber = "T0" + tableId, DiningArea = "Main Dining" };

            return Page();
        }
    }
}