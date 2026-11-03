using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationSystem.Models;

public class Reservation
{
    public int ReservationId { get; set; }

    public string CustomerId { get; set; } = "";

    [Required]
    public DateTime ReservationDate { get; set; }

    [Range(1, 20)]
    public int PartySize { get; set; }

    public int? RestaurantTableId { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    public RestaurantTable? RestaurantTable { get; set; }
    public List<FoodOrder> Orders { get; set; } = new();
}