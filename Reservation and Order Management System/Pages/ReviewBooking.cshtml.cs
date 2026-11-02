using Microsoft.AspNetCore.Mvc.RazorPages;

public class ReviewBookingModel : PageModel
{
    public string BookingId { get; set; }
    public string ReservationDate { get; set; }
    public string ReservationTime { get; set; }
    public int GuestCount { get; set; }
    public string TableNumber { get; set; }

    public void OnGet()
    {

    }
}