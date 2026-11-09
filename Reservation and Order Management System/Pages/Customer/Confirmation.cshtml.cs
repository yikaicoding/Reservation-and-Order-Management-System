using Microsoft.AspNetCore.Mvc.RazorPages;

public class ConfirmationModel : PageModel
{
    public string BookingId { get; set; }
    public string ReservationDate { get; set; }
    public string ReservationTime { get; set; }
    public string TableNumber { get; set; }

    public void OnGet()
    {

    }
}