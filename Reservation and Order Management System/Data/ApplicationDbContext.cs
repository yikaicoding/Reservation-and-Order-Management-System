using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantReservationSystem.Models;

namespace Reservation_and_Order_Management_System.Data;

public class ApplicationDbContext
    : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerProfile> CustomerProfiles =>
        Set<CustomerProfile>();

    public DbSet<RestaurantTable> DiningTables =>
        Set<RestaurantTable>();

    public DbSet<Reservation> Reservations =>
        Set<Reservation>();

    public DbSet<FoodOrder> FoodOrders => 
        Set<FoodOrder>();

    public DbSet<FoodOrderItem> FoodOrderItems => 
        Set<FoodOrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<FoodOrder>()
            .HasIndex(order => order.ReservationId)
            .IsUnique();

        builder.Entity<FoodOrder>()
            .HasOne(order => order.Reservation)
            .WithOne(reservation => reservation.FoodOrder)
            .HasForeignKey<FoodOrder>(order => order.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FoodOrderItem>()
            .HasOne(item => item.FoodOrder)
            .WithMany(order => order.Items)
            .HasForeignKey(item => item.FoodOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Reservation>()
            .HasOne(reservation => reservation.CustomerProfile)
            .WithMany(profile => profile.Reservations)
            .HasForeignKey(reservation => reservation.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reservation>()
            .HasOne(reservation => reservation.CustomerProfile)
            .WithMany(profile => profile.Reservations)
            .HasForeignKey(reservation => reservation.CustomerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reservation>()
            .HasIndex(reservation => new
            {
                reservation.RestaurantTableId,
                reservation.StartsAt
            });
    }
}