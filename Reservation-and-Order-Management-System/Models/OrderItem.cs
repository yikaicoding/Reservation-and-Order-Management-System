using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationSystem.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }

    public int FoodOrderId { get; set; }

    [Required, StringLength(100)]
    public string FoodName { get; set; } = "";

    [Range(1, 100)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "99999")]
    public decimal UnitPrice { get; set; }

    public FoodOrder? FoodOrder { get; set; }
}