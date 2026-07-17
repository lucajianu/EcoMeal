using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMeal.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Businesses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Businesses");
        }
    }
}
