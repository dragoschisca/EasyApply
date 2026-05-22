using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyApply.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyCultureToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyCulture",
                table: "Companies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyCulture",
                table: "Companies");
        }
    }
}
