using System;
using System.ComponentModel.DataAnnotations;

namespace Reservation_and_Order_Management_System.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int RestaurantTableId { get; set; }

        public DateTime ReservationDate { get; set; }

        public string TimeSlot { get; set; } = string.Empty;

        public int NumberOfGuests { get; set; }

        public string? SpecialRequests { get; set; }

        public string Status { get; set; } = "Confirmed";

        // Navigation property
        public RestaurantTable? RestaurantTable { get; set; }
    }
}
