using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using RestaurantReservationSystem.Models;

namespace Reservation_and_Order_Management_System.Pages.Reservations;

[Authorize]
public class CreateModel(ApplicationDbContext db) : PageModel
{
    [BindProperty]
    public ReservationInput Input { get; set; } = new();

    public List<RestaurantTable> Tables { get; set; } = new();

    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task OnGetAsync()
    {
        var defaultStart = DateTime.Now.AddMinutes(30);
        Input.StartsAt = defaultStart.AddTicks(
            -(defaultStart.Ticks % TimeSpan.TicksPerMinute));

        await LoadTablesAsync();
    }

    private async Task LoadTablesAsync()
    {
        Tables = await db.DiningTables
            .Where(table => table.IsActive)
            .OrderBy(table => table.TableName)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadTablesAsync();

        if (!ModelState.IsValid)
            return Page();

        var profile = await db.CustomerProfiles
            .SingleOrDefaultAsync(customer => customer.CustomerId == UserId);

        if (profile is null)
        {
            ModelState.AddModelError(
                "",
                "Complete your customer profile before booking.");

            return Page();
        }

        var table = await db.DiningTables
            .SingleOrDefaultAsync(table =>
                table.RestaurantTableId == Input.RestaurantTableId &&
                table.IsActive);

        if (table is null || table.Capacity < Input.PartySize)
        {
            ModelState.AddModelError(
                "",
                "Choose an active table that fits your party.");

            return Page();
        }

        var start = Input.StartsAt;
        var end = start.AddHours(2);

        if (start <= DateTime.Now.AddMinutes(15))
        {
            ModelState.AddModelError(
                "Input.StartsAt",
                "Choose a start time at least 15 minutes from now.");

            return Page();
        }

        var reservation = new Reservation
        {
            CustomerId = profile.CustomerId,
            RestaurantTableId = table.RestaurantTableId,
            StartsAt = start,
            EndsAt = end,
            PartySize = Input.PartySize,
            Status = "Pending"
        };

        var strategy = db.Database.CreateExecutionStrategy();
        var overlaps = false;

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await db.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            overlaps = await db.Reservations.AnyAsync(existing =>
                existing.RestaurantTableId == table.RestaurantTableId &&
                existing.Status != "Cancelled" &&
                existing.StartsAt < end &&
                existing.EndsAt > start);

            if (overlaps)
            {
                await transaction.RollbackAsync();
                return;
            }

            db.Reservations.Add(reservation);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        });

        if (overlaps)
        {
            ModelState.AddModelError(
                "",
                "That table is already booked during this time. Choose another time or table.");

            return Page();
        }

        TempData["Message"] = "Reservation submitted. Staff must confirm it.";
        return RedirectToPage();
    }

    public class ReservationInput
    {
        [Required]
        public DateTime StartsAt { get; set; }

        [Range(1, 20)]
        public int PartySize { get; set; }

        [Range(1, int.MaxValue)]
        public int RestaurantTableId { get; set; }
    }
}