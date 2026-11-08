using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using RestaurantReservationSystem.Models;

namespace Reservation_and_Order_Management_System.Pages.Staff;

[Authorize(Roles = "Staff,Admin")]
public class ReservationsModel(ApplicationDbContext db) : PageModel
{
    public List<Reservation> Reservations { get; private set; } = [];
    public List<RestaurantTable> Tables { get; private set; } = [];
    [BindProperty] public ReservationUpdate Input { get; set; } = new();

    public async Task OnGetAsync(string? status) => await LoadAsync(status);

    public async Task<IActionResult> OnPostConfirmAsync(int id)
    {
        var reservation = await db.Reservations.FindAsync(id);
        if (reservation is null) return NotFound();
        if (reservation.Status != "Pending")
        {
            TempData["Message"] = "Only pending reservations can be confirmed.";
            return RedirectToPage();
        }
        reservation.Status = "Confirmed";
        await db.SaveChangesAsync();
        TempData["Message"] = $"Reservation #{id} confirmed.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var reservation = await db.Reservations.FindAsync(id);
        if (reservation is null) return NotFound();
        if (reservation.Status == "Cancelled")
        {
            TempData["Message"] = $"Reservation #{id} is already cancelled.";
            return RedirectToPage();
        }
        reservation.Status = "Cancelled";
        await db.SaveChangesAsync();
        TempData["Message"] = $"Reservation #{id} cancelled.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostModifyAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }
        var reservation = await db.Reservations.SingleOrDefaultAsync(item => item.ReservationId == Input.ReservationId);
        if (reservation is null) return NotFound();
        if (reservation.Status is not ("Pending" or "Confirmed"))
        {
            TempData["Message"] = "Only pending or confirmed reservations can be changed.";
            return RedirectToPage();
        }
        var table = await db.DiningTables.SingleOrDefaultAsync(item => item.RestaurantTableId == Input.RestaurantTableId && item.IsActive);
        if (table is null || table.Capacity < Input.PartySize)
        {
            ModelState.AddModelError("Input.RestaurantTableId", "Choose an active table that fits the party.");
            await LoadAsync();
            return Page();
        }

        var startsAt = DateTime.SpecifyKind(Input.StartsAt, DateTimeKind.Local);
        var endsAt = startsAt.AddHours(2);
        var strategy = db.Database.CreateExecutionStrategy();
        var unavailable = false;
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            unavailable = await db.Reservations.AnyAsync(existing => existing.ReservationId != reservation.ReservationId &&
                existing.RestaurantTableId == table.RestaurantTableId && existing.Status != "Cancelled" &&
                existing.StartsAt < endsAt && existing.EndsAt > startsAt);
            if (unavailable)
            {
                await transaction.RollbackAsync();
                return;
            }
            reservation.RestaurantTableId = table.RestaurantTableId;
            reservation.StartsAt = startsAt;
            reservation.EndsAt = endsAt;
            reservation.PartySize = Input.PartySize;
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        });
        if (unavailable)
        {
            ModelState.AddModelError("", "That table is already reserved during this time. Choose another table or time.");
            await LoadAsync();
            return Page();
        }
        TempData["Message"] = $"Reservation #{reservation.ReservationId} updated.";
        return RedirectToPage();
    }

    private async Task LoadAsync(string? status = null)
    {
        var query = db.Reservations.AsNoTracking().Include(item => item.CustomerProfile)
            .Include(item => item.RestaurantTable).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(item => item.Status == status);
        Reservations = await query.OrderBy(item => item.StartsAt).ThenBy(item => item.ReservationId).ToListAsync();
        Tables = await db.DiningTables.AsNoTracking().Where(item => item.IsActive).OrderBy(item => item.TableName).ToListAsync();
    }

    public class ReservationUpdate
    {
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)] public int ReservationId { get; set; }
        [System.ComponentModel.DataAnnotations.Required] public DateTime StartsAt { get; set; }
        [System.ComponentModel.DataAnnotations.Range(1, 20)] public int PartySize { get; set; }
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)] public int RestaurantTableId { get; set; }
    }
}
