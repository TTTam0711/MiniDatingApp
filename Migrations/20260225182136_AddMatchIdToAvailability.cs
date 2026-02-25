using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniDatingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchIdToAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "Availabilities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "Availabilities");
        }
    }
}
