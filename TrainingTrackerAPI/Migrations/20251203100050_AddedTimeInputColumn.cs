using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimeInputColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeInput",
                table: "Activities",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeInput",
                table: "Activities");
        }
    }
}
