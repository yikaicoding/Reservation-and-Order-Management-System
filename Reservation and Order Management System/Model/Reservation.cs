using RestaurantReservationSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationSystem.Models;

public class Reservation
{
    public int ReservationId { get; set; }

    [Required]
    public string CustomerId { get; set; }

    public CustomerProfile? CustomerProfile { get; set; }

    public int RestaurantTableId { get; set; }

    public RestaurantTable? RestaurantTable { get; set; }

    [Required]
    public DateTime StartsAt { get; set; }

    [Required]
    public DateTime EndsAt { get; set; }

    [Range(1, 20)]
    public int PartySize { get; set; }

    [Required, StringLength(20)]
    public string Status { get; set; } = "Pending";

    public FoodOrder? FoodOrder { get; set; }
}