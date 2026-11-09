using Microsoft.AspNetCore.Mvc.RazorPages;

public class BookingSummaryModel : PageModel
{
    public string ReservationDate { get; set; }
    public string ReservationTime { get; set; }
    public int GuestCount { get; set; }
    public string TableNumber { get; set; }
    public string SpecialRequest { get; set; }
    public string Status { get; set; }

    public void OnGet()
    {
       
    }
}
