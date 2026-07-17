using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMeal.Backend.Migrations
{
    /// <inheritdoc />
    public partial class OrderStatusEnumAndStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // comenzile vechi aveau statusul "Placed", care nu exista in enum-ul State
            migrationBuilder.Sql("UPDATE Orders SET Status = 'Pending' WHERE Status NOT IN ('Pending','Accepted','Rejected','Finished','Cancelled')");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
