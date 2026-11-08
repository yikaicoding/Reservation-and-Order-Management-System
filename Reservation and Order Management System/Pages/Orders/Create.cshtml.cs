using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using RestaurantReservationSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Reservation_and_Order_Management_System.Pages.Orders;

[Authorize]
public class CreateModel(ApplicationDbContext db) : PageModel
{
    private const int MaximumOrderLines = 3;

    [BindProperty, Range(1, int.MaxValue)]
    public int ReservationId { get; set; }

    [BindProperty]
    public List<OrderLineInput> Lines { get; set; } = [new(), new(), new()];

    public List<Reservation> Reservations { get; set; } = [];

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task OnGetAsync()
    {
        await LoadReservationsAsync();
    }

    private async Task LoadReservationsAsync()
    {
        Reservations = await db.Reservations
            .Where(reservation =>
                reservation.CustomerId == UserId &&
                reservation.Status == "Confirmed" &&
                reservation.StartsAt > DateTime.Now &&
                reservation.FoodOrder == null)
            .OrderBy(reservation => reservation.StartsAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadReservationsAsync();

        var selectedReservation = await db.Reservations
            .SingleOrDefaultAsync(reservation =>
                reservation.ReservationId == ReservationId &&
                reservation.CustomerId == UserId &&
                reservation.Status == "Confirmed" &&
                reservation.StartsAt > DateTime.Now &&
                reservation.FoodOrder == null);

        if (selectedReservation is null)
        {
            ModelState.AddModelError(
                nameof(ReservationId),
                "Select one of your future confirmed reservations that does not already have an order.");
        }

        Lines ??= [];

        if (Lines.Count > MaximumOrderLines)
        {
            ModelState.AddModelError(
                nameof(Lines),
                $"An order can contain at most {MaximumOrderLines} items.");
        }

        var items = Lines
            .Where(line => line is not null &&
                           !string.IsNullOrWhiteSpace(line.ItemName))
            .ToList();

        if (items.Count == 0)
        {
            ModelState.AddModelError("", "Enter at least one food item.");
        }

        if (items.Any(line =>
                line.ItemName.Trim().Length > 100 ||
                line.Quantity < 1 ||
                line.Quantity > 50))
        {
            ModelState.AddModelError(
                "",
                "Each item needs a name no longer than 100 characters and a quantity from 1 to 50.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var order = new FoodOrder
        {
            ReservationId = selectedReservation!.ReservationId,
            Items = items.Select(line => new FoodOrderItem
            {
                ItemName = line.ItemName.Trim(),
                Quantity = line.Quantity
            }).ToList()
        };

        db.FoodOrders.Add(order);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "",
                "An order has already been submitted for this reservation. Choose another reservation.");

            await LoadReservationsAsync();
            return Page();
        }

        TempData["Message"] = "Food order submitted and linked to your reservation.";
        return RedirectToPage("/Index");
    }

    public class OrderLineInput
    {
        [StringLength(100)]
        public string ItemName { get; set; } = "";

        [Range(1, 50)]
        public int Quantity { get; set; } = 1;
    }
}
