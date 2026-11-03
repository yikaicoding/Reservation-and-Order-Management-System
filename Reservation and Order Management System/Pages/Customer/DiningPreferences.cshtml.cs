using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Pages.Customer
{
    //US06 - Dining preferences & special requests (Booking step 3)
    public class DiningPreferencesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DiningPreferencesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        //Choices shown as chips on the page
        public static readonly string[] SeatingOptions =
            { "Window table", "Quiet area", "Garden patio", "Near the bar" };

        public static readonly string[] OccasionOptions =
            { "Birthday", "Anniversary", "Business", "Date night", "Family gathering" };

        public static readonly string[] DietaryOptions =
            { "Vegetarian", "Halal", "Gluten-free", "No peanuts", "Shellfish allergy" };


        //Booking details from step 1 & 2 (Member 3's pages)
        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }

        [BindProperty(SupportsGet = true)]
        public TimeSpan Time { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Guests { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TableId { get; set; }


        //US06 inputs
        [BindProperty]
        public string? SeatingPreference { get; set; }

        [BindProperty]
        public string? Occasion { get; set; }

        [BindProperty]
        public List<string> Dietary { get; set; } = new List<string>();

        [BindProperty]
        [StringLength(100, ErrorMessage = "Other dietary needs cannot be more than 100 characters.")]
        public string? OtherDietary { get; set; }

        [BindProperty]
        [StringLength(500, ErrorMessage = "Special requests cannot be more than 500 characters.")]
        public string? SpecialRequests { get; set; }

        public string? ErrorMessage { get; set; }


        public IActionResult OnGet()
        {
            //Must be logged in
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return RedirectToPage("/Customer/Login");
            }

            CheckBookingDetails();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Must be logged in
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToPage("/Customer/Login");
            }

            //Booking details must still be valid
            if (!CheckBookingDetails())
            {
                return Page();
            }

            //Only accept choices from our lists (stops tampered forms)
            if (!string.IsNullOrEmpty(SeatingPreference) &&
                !SeatingOptions.Contains(SeatingPreference))
            {
                SeatingPreference = null;
            }

            if (!string.IsNullOrEmpty(Occasion) &&
                !OccasionOptions.Contains(Occasion))
            {
                Occasion = null;
            }

            //Combine dietary chips + "other" text into one line
            var dietaryList = Dietary
                .Where(d => DietaryOptions.Contains(d))
                .ToList();

            if (!string.IsNullOrWhiteSpace(OtherDietary))
            {
                dietaryList.Add(OtherDietary.Trim());
            }

            string? dietaryText = dietaryList.Count > 0
                ? string.Join(", ", dietaryList)
                : null;

            if (dietaryText != null && dietaryText.Length > 200)
            {
                ModelState.AddModelError("OtherDietary", "Dietary requirements are too long.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Save the reservation as Pending (confirmed in step 4)
            var reservation = new Reservation
            {
                CustomerId = customerId.Value,
                RestaurantTableId = TableId,
                ReservationDate = Date.Date,
                ReservationTime = Time,
                NumberOfGuests = Guests,
                Status = "Pending",
                SeatingPreference = SeatingPreference,
                Occasion = Occasion,
                DietaryRequirements = dietaryText,
                SpecialRequests = string.IsNullOrWhiteSpace(SpecialRequests)
                    ? null
                    : SpecialRequests.Trim()
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            //Go to step 4 - Confirm Booking (Member 4, US07)
            return RedirectToPage("/Customer/ConfirmBooking", new { id = reservation.ReservationId });
        }


        //Checks the date, time, guests and table passed from step 1 & 2
        private bool CheckBookingDetails()
        {
            if (TableId <= 0 || Guests < 1 || Guests > 20 || Date == DateTime.MinValue)
            {
                ErrorMessage = "Your booking details are missing. Please start again from Reservation.";
                return false;
            }

            if (Date.Date.Add(Time) < DateTime.Now)
            {
                ErrorMessage = "That date and time has already passed. Please pick another slot.";
                return false;
            }

            return true;
        }
    }
}