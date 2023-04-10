using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenJob.Migrations
{
    /// <inheritdoc />
    public partial class _70to71 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "OrleansMembershipVersionTable",
                type: "DATETIME2(3)",
                precision: 3,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME2(3)",
                oldPrecision: 3,
                oldDefaultValue: new DateTime(2023, 3, 2, 2, 23, 34, 657, DateTimeKind.Utc).AddTicks(5118));

            migrationBuilder.AddColumn<int>(
                name: "EntityVersion",
                table: "AbpUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntityVersion",
                table: "AbpTenants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntityVersion",
                table: "AbpRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntityVersion",
                table: "AbpOrganizationUnits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityVersion",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "EntityVersion",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "EntityVersion",
                table: "AbpRoles");

            migrationBuilder.DropColumn(
                name: "EntityVersion",
                table: "AbpOrganizationUnits");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "OrleansMembershipVersionTable",
                type: "DATETIME2(3)",
                precision: 3,
                nullable: false,
                defaultValue: new DateTime(2023, 3, 2, 2, 23, 34, 657, DateTimeKind.Utc).AddTicks(5118),
                oldClrType: typeof(DateTime),
                oldType: "DATETIME2(3)",
                oldPrecision: 3);
        }
    }
}
