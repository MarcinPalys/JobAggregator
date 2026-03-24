using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Company = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SalaryMin = table.Column<decimal>(type: "numeric", nullable: true),
                    SalaryMax = table.Column<decimal>(type: "numeric", nullable: true),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    SourceName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOffers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobOffers_ExternalId_SourceName",
                table: "JobOffers",
                columns: new[] { "ExternalId", "SourceName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobOffers");
        }
    }
}
