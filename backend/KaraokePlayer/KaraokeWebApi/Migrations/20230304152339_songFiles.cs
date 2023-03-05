using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KaraokeWebApi.Migrations
{
    /// <inheritdoc />
    public partial class songFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Working",
                table: "Songs");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Songs",
                newName: "CdgFile");

            migrationBuilder.AddColumn<string>(
                name: "Mp3File",
                table: "Songs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mp3File",
                table: "Songs");

            migrationBuilder.RenameColumn(
                name: "CdgFile",
                table: "Songs",
                newName: "FilePath");

            migrationBuilder.AddColumn<bool>(
                name: "Working",
                table: "Songs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
