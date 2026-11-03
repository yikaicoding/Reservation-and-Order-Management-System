using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationSystem.Models;

public class FoodOrder
{
    public int FoodOrderId { get; set; }

    public int ReservationId { get; set; }

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "Submitted";

    public Reservation? Reservation { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}