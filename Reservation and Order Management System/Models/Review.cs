using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reservation_and_Order_Management_System.Models
{
    //US12 - Customer dining review & star ratings
    //One review per reservation (unique index)
    [Index(nameof(ReservationId), IsUnique = true)]
    public class Review
    {
        public int ReviewId { get; set; }

        //Which visit this review is for (customer is found through the reservation)
        [Required]
        public int ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }

        [Required]
        [Range(1, 5)]
        public int FoodRating { get; set; }

        [Required]
        [Range(1, 5)]
        public int ServiceRating { get; set; }

        [Required]
        [Range(1, 5)]
        public int AmbienceRating { get; set; }

        [Required]
        [Range(1, 5)]
        public int OverallRating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}