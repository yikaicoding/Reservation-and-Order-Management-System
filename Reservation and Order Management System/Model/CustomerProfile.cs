using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationSystem.Models;

public class CustomerProfile
{
    [Key]
    [Required]
    public string CustomerId { get; set; } = "";

    [Required, StringLength(100)]
    public string FullName { get; set; } = "";

    [EmailAddress]
    public string Email { get; set; } = "";

    [Phone]
    public string PhoneNumber { get; set; } = "";

    [StringLength(500)]
    public string? DietaryPreferences { get; set; }

    [StringLength(500)]
    public string? DiningPreferences { get; set; }

    public List<Reservation> Reservations { get; set; } = new();
}