using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US12 - Customer dining review, star ratings & feedback
    public class ReviewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ReviewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Reservation Reservation { get; set; } = null!;

        [BindProperty]
        [Range(1, 5, ErrorMessage = "Please rate the food.")]
        public int FoodRating { get; set; }

        [BindProperty]
        [Range(1, 5, ErrorMessage = "Please rate the service.")]
        public int ServiceRating { get; set; }

        [BindProperty]
        [Range(1, 5, ErrorMessage = "Please rate the ambience.")]
        public int AmbienceRating { get; set; }

        [BindProperty]
        [Range(1, 5, ErrorMessage = "Please give an overall rating.")]
        public int OverallRating { get; set; }

        [BindProperty]
        [StringLength(500, ErrorMessage = "Comment cannot be more than 500 characters.")]
        public string? Comment { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var check = await CheckReservation(id);
            if (check != null)
            {
                return check;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var check = await CheckReservation(id);
            if (check != null)
            {
                return check;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Save the review
            var review = new Review
            {
                ReservationId = id,
                FoodRating = FoodRating,
                ServiceRating = ServiceRating,
                AmbienceRating = AmbienceRating,
                OverallRating = OverallRating,
                Comment = string.IsNullOrWhiteSpace(Comment) ? null : Comment.Trim()
            };

            _context.Reviews.Add(review);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                //Unique index stopped a double submit
                TempData["SuccessMessage"] = "You already reviewed this visit.";
                return RedirectToPage("/Customer/MyReservation");
            }

            TempData["SuccessMessage"] = "Thank you for your feedback!";
            return RedirectToPage("/Customer/MyReservation");
        }

        //Checks: logged in, own reservation, visit has passed, not cancelled, not reviewed yet
        private async Task<IActionResult?> CheckReservation(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToPage("/Customer/Login");
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.ReservationId == id && r.CustomerId == customerId);

            //Also hides other customers' reservations
            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status == "Cancelled" ||
                reservation.ReservationDate.Date.Add(reservation.ReservationTime) > DateTime.Now)
            {
                TempData["SuccessMessage"] = "You can only review a visit after it has happened.";
                return RedirectToPage("/Customer/MyReservation");
            }

            bool alreadyReviewed = await _context.Reviews
                .AnyAsync(rv => rv.ReservationId == id);

            if (alreadyReviewed)
            {
                TempData["SuccessMessage"] = "You already reviewed this visit.";
                return RedirectToPage("/Customer/MyReservation");
            }

            Reservation = reservation;
            return null;
        }
    }
}