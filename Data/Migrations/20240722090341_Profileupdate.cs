using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class Profileupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "Profiles");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "Profiles",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Profiles");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
