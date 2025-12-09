using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsCadenceAndPower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvgCadence",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AvgPower",
                table: "Activities",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgCadence",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "AvgPower",
                table: "Activities");
        }
    }
}
