using Microsoft.EntityFrameworkCore;
using RestaurantReservationSystem.Models;

namespace Reservation_and_Order_Management_System.Data;

public static class DbSeeder
{
    public static async Task SeedTablesAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        var sampleTables = new[]
        {
            (Name: "Table 1", Capacity: 2),
            (Name: "Table 2", Capacity: 2),
            (Name: "Table 3", Capacity: 4),
            (Name: "Table 4", Capacity: 4),
            (Name: "Table 5", Capacity: 6)
        };

        var changed = false;

        foreach (var sample in sampleTables)
        {
            var existingTable = await db.DiningTables
                .SingleOrDefaultAsync(table => table.TableName == sample.Name);

            if (existingTable is null)
            {
                db.DiningTables.Add(new RestaurantTable
                {
                    TableName = sample.Name,
                    Capacity = sample.Capacity,
                    IsActive = true
                });

                changed = true;
            }
            else if (!existingTable.IsActive)
            {
                existingTable.IsActive = true;
                changed = true;
            }
        }

        if (changed)
        {
            await db.SaveChangesAsync();
        }
    }
}