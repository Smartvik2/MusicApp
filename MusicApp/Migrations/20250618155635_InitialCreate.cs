using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Appointments",
                newName: "UserNote");

            migrationBuilder.AddColumn<string>(
                name: "ArtistNote",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtistNote",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "UserNote",
                table: "Appointments",
                newName: "Notes");
        }
    }
}
