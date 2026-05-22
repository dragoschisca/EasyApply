using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyApply.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressAndGitHubUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubUrl",
                table: "Candidates",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "GitHubUrl",
                table: "Candidates");
        }
    }
}
