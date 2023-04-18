using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenJob.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "OpenJobWorkerInfo",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "OpenJobWorkerInfo");
        }
    }
}
