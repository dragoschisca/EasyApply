using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyApply.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJobLocationAndCulture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemote",
                table: "Jobs");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCulture",
                table: "Jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationType",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyCulture",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LocationType",
                table: "Jobs");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemote",
                table: "Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
