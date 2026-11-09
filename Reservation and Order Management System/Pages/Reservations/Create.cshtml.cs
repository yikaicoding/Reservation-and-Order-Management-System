using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Data;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Pages.Reservations;

public class CreateModel(ApplicationDbContext db, IWebHostEnvironment environment) : PageModel
{
    private const long MaximumGalleryImageSize = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    [BindProperty]
    public ReservationInput Input { get; set; } = new();

    public List<RestaurantTable> Tables { get; set; } = new();

    public List<string> GalleryImages { get; set; } = [];
    public bool CanManageGallery => User.IsInRole("Staff");

    //Customer from our session login (US01)
    private int UserId => HttpContext.Session.GetInt32("CustomerId") ?? 0;

    private string GalleryDirectory =>
        Path.Combine(environment.WebRootPath, "uploads", "restaurant-gallery");


    public async Task<IActionResult> OnGetAsync()
    {
        if (UserId == 0 && !CanManageGallery)
        {
            return RedirectToPage("/Customer/Login");
        }

        var defaultStart = DateTime.Now.AddMinutes(30);
        Input.StartsAt = defaultStart.AddTicks(
            -(defaultStart.Ticks % TimeSpan.TicksPerMinute));

        await LoadTablesAsync();
        LoadGalleryImages();
        return Page();
    }

    private void LoadGalleryImages()
    {
        if (!Directory.Exists(GalleryDirectory))
        {
            GalleryImages = [];
            return;
        }

        GalleryImages = Directory.EnumerateFiles(GalleryDirectory)
            .Where(path => AllowedImageExtensions.Contains(Path.GetExtension(path).ToLowerInvariant()))
            .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
            .Select(path => $"/uploads/restaurant-gallery/{Path.GetFileName(path)}")
            .ToList();
    }

    public async Task<IActionResult> OnPostUploadImageAsync(IFormFile? image)
    {
        if (!CanManageGallery)
        {
            return Forbid();
        }

        if (image is null || image.Length == 0)
        {
            TempData["UploadError"] = "Choose an image file to upload.";
            return RedirectToPage();
        }

        if (image.Length > MaximumGalleryImageSize)
        {
            TempData["UploadError"] = "Image files must be 5 MB or smaller.";
            return RedirectToPage();
        }

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (!AllowedImageExtensions.Contains(extension) || !await HasValidImageSignatureAsync(image, extension))
        {
            TempData["UploadError"] = "Upload a valid JPG, PNG, or WebP image.";
            return RedirectToPage();
        }

        Directory.CreateDirectory(GalleryDirectory);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var destination = Path.Combine(GalleryDirectory, fileName);

        await using (var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await image.CopyToAsync(output);
        }

        TempData["UploadMessage"] = "Image uploaded to the restaurant gallery.";
        return RedirectToPage();
    }

    private static async Task<bool> HasValidImageSignatureAsync(IFormFile image, string extension)
    {
        var header = new byte[12];
        await using var input = image.OpenReadStream();
        var bytesRead = 0;

        while (bytesRead < header.Length)
        {
            var count = await input.ReadAsync(header.AsMemory(bytesRead));
            if (count == 0) break;
            bytesRead += count;
        }

        return extension switch
        {
            ".jpg" or ".jpeg" => bytesRead >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => bytesRead >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            ".webp" => bytesRead >= 12 && header.AsSpan(0, 4).SequenceEqual("RIFF"u8) && header.AsSpan(8, 4).SequenceEqual("WEBP"u8),
            _ => false
        };
    }

    private async Task LoadTablesAsync()
    {
        Tables = await db.RestaurantTables
            .Where(table => table.IsActive)
            .OrderBy(table => table.TableName)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        //Must be logged in as a customer
        if (UserId == 0)
        {
            return RedirectToPage("/Customer/Login");
        }

        await LoadTablesAsync();
        LoadGalleryImages();

        if (!ModelState.IsValid)
            return Page();

        var table = await db.RestaurantTables
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
            CustomerId = UserId,
            RestaurantTableId = table.RestaurantTableId,
            StartsAt = start,
            EndsAt = end,
            PartySize = Input.PartySize,
            //Fields used by Order History / Dashboard (US11)
            ReservationDate = start.Date,
            ReservationTime = start.TimeOfDay,
            NumberOfGuests = Input.PartySize,
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