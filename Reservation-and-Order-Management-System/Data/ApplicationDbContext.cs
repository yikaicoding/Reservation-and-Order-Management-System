using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantReservationSystem.Models;

namespace RestaurantReservationSystem.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerProfile> CustomerProfiles =>
        Set<CustomerProfile>();

    public DbSet<RestaurantTable> RestaurantTables =>
        Set<RestaurantTable>();

    public DbSet<Reservation> Reservations =>
        Set<Reservation>();

    public DbSet<FoodOrder> FoodOrders =>
        Set<FoodOrder>();

    public DbSet<OrderItem> OrderItems =>
        Set<OrderItem>();
}