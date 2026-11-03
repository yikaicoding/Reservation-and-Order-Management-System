using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationSystem.Models;

public class RestaurantTable
{
    public int RestaurantTableId { get; set; }

    [Required]
    public string TableName { get; set; } = "";

    [Range(1, 20)]
    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;

    public List<Reservation> Reservations { get; set; } = new();
}