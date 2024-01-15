using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroScan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedScans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageBase64 = table.Column<string>(type: "text", nullable: false),
                    PlantUri = table.Column<string>(type: "text", nullable: false),
                    PlantName = table.Column<string>(type: "text", nullable: false),
                    DiseaseUri = table.Column<string>(type: "text", nullable: true),
                    DiseaseName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scans", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scans");
        }
    }
}
