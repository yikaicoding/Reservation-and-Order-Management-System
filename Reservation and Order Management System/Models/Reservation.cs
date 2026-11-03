using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservation_and_Order_Management_System.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        //Which customer made the booking
        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        //Which table (Member 3 will link this to RestaurantTable later)
        [Required]
        public int RestaurantTableId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime ReservationDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Time")]
        public TimeSpan ReservationTime { get; set; }

        [Required]
        [Range(1, 20)]
        [Display(Name = "No. of Pax")]
        public int NumberOfGuests { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Reserved";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //US06 - Dining preferences & special requests (Member 1)
        [StringLength(50)]
        [Display(Name = "Seating Preference")]
        public string? SeatingPreference { get; set; }

        [StringLength(50)]
        [Display(Name = "Special Occasion")]
        public string? Occasion { get; set; }

        [StringLength(200)]
        [Display(Name = "Dietary Requirements / Allergies")]
        public string? DietaryRequirements { get; set; }

        [StringLength(500)]
        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }
    }
}