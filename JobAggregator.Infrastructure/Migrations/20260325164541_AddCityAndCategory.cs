using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCityAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "JobOffers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "JobOffers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "JobOffers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "JobOffers");
        }
    }
}
