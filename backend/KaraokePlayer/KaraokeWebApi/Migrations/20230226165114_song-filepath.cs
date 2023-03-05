using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KaraokeWebApi.Migrations
{
    /// <inheritdoc />
    public partial class songfilepath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Songs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Songs");
        }
    }
}
