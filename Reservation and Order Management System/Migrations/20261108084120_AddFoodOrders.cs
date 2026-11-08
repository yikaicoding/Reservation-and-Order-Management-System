using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation_and_Order_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_DiningTables_RestaurantTableId",
                table: "Reservations");

            migrationBuilder.CreateTable(
                name: "FoodOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodOrders_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodOrderId = table.Column<int>(type: "int", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodOrderItems_FoodOrders_FoodOrderId",
                        column: x => x.FoodOrderId,
                        principalTable: "FoodOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrderItems_FoodOrderId",
                table: "FoodOrderItems",
                column: "FoodOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrders_ReservationId",
                table: "FoodOrders",
                column: "ReservationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_DiningTables_RestaurantTableId",
                table: "Reservations",
                column: "RestaurantTableId",
                principalTable: "DiningTables",
                principalColumn: "RestaurantTableId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_DiningTables_RestaurantTableId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "FoodOrderItems");

            migrationBuilder.DropTable(
                name: "FoodOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_DiningTables_RestaurantTableId",
                table: "Reservations",
                column: "RestaurantTableId",
                principalTable: "DiningTables",
                principalColumn: "RestaurantTableId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
