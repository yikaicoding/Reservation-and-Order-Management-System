using System.ComponentModel.DataAnnotations;

namespace RestaurantReservationSystem.Models.ProfileViewModel;

public class ProfileViewModel
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = "";

    [EmailAddress, StringLength(200)]
    public string Email { get; set; } = "";

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? DietaryPreferences { get; set; }

    [StringLength(500)]
    public string? AccessibilityNotes { get; set; }
}