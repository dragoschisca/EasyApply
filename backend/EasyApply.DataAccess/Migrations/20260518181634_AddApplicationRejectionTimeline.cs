using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyApply.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationRejectionTimeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "rejection_feedback",
                table: "Applications",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "application_status_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    feedback = table.Column<string>(type: "text", nullable: true),
                    changed_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_application_status_history_Applications_application_id",
                        column: x => x.application_id,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_status_history_application_id_changed_at",
                table: "application_status_history",
                columns: new[] { "application_id", "changed_at" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_status_history");

            migrationBuilder.DropColumn(
                name: "rejection_feedback",
                table: "Applications");
        }
    }
}
