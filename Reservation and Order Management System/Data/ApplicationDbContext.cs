using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        public DbSet<RestaurantTable> RestaurantTables { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
    }
}