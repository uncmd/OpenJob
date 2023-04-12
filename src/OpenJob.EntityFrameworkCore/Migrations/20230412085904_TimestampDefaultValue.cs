using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenJob.Migrations
{
    /// <inheritdoc />
    public partial class TimestampDefaultValue : Migration
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
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "DATETIME2(3)",
                oldPrecision: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                oldDefaultValueSql: "getdate()");
        }
    }
}
