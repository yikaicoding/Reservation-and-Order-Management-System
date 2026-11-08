using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Reservation_and_Order_Management_System.Pages.Customer;

public class DiningPreferencesModel : PageModel
{
    private const string SessionKey = "DiningPreferences";

    [BindProperty]
    public PreferenceInput Input { get; set; } = new();

    public string? Message { get; private set; }

    public void OnGet()
    {
        var saved = HttpContext.Session.GetString(SessionKey);

        if (!string.IsNullOrEmpty(saved))
        {
            Input = JsonSerializer.Deserialize<PreferenceInput>(saved) ?? new();
        }
    }

    public void OnPost()
    {
        if (!ModelState.IsValid)
        {
            return;
        }

        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(Input));
        Message = "Your dining preferences have been saved.";
    }

    public class PreferenceInput
    {
        [Display(Name = "Vegetarian")]
        public bool Vegetarian { get; set; }

        [Display(Name = "Vegan")]
        public bool Vegan { get; set; }

        [Display(Name = "Halal")]
        public bool Halal { get; set; }

        [Display(Name = "Gluten-free")]
        public bool GlutenFree { get; set; }

        [StringLength(500)]
        [Display(Name = "Allergies")]
        public string? Allergies { get; set; }

        [StringLength(1000)]
        [Display(Name = "Special requests")]
        public string? SpecialRequests { get; set; }
    }
}