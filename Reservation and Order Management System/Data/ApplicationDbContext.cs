using Microsoft.EntityFrameworkCore;
using Reservation_and_Order_Management_System.Models;

namespace Reservation_and_Order_Management_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RestaurantTable> RestaurantTables { get; set; }
    }
}
