using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationSystem.Models;

public class FoodOrder
{
    public int Id { get; set; }

    [Required]
    public int ReservationId { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    [Required, StringLength(20)]
    public string Status { get; set; } = "Submitted";

    public Reservation? Reservation { get; set; }

    public List<FoodOrderItem> Items { get; set; } = [];
}

public class FoodOrderItem
{
    public int Id { get; set; }

    [Required]
    public int FoodOrderId { get; set; }

    [Required, StringLength(100)]
    public string ItemName { get; set; } = "";

    [Range(1, 50)]
    public int Quantity { get; set; }

    public FoodOrder? FoodOrder { get; set; }
}