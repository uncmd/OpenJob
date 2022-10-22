using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerScheduler.Migrations
{
    public partial class Added_Orleans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrleansMembershipVersionTable",
                columns: table => new
                {
                    DeploymentId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "DATETIME2(3)", precision: 3, nullable: false, defaultValue: new DateTime(2022, 10, 22, 9, 3, 0, 302, DateTimeKind.Utc).AddTicks(8693)),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrleansMembershipVersionTable", x => x.DeploymentId);
                });

            migrationBuilder.CreateTable(
                name: "OrleansQuery",
                columns: table => new
                {
                    QueryKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    QueryText = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrleansQuery", x => x.QueryKey);
                });

            migrationBuilder.CreateTable(
                name: "OrleansRemindersTable",
                columns: table => new
                {
                    ServiceId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GrainId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReminderName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StartTime = table.Column<DateTime>(type: "DATETIME2(3)", precision: 3, nullable: false),
                    Period = table.Column<long>(type: "bigint", nullable: false),
                    GrainHash = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrleansRemindersTable", x => new { x.ServiceId, x.GrainId, x.ReminderName });
                });

            migrationBuilder.CreateTable(
                name: "OrleansStorage",
                columns: table => new
                {
                    GrainIdHash = table.Column<int>(type: "int", nullable: false),
                    GrainIdN0 = table.Column<long>(type: "bigint", nullable: false),
                    GrainIdN1 = table.Column<long>(type: "bigint", nullable: false),
                    GrainTypeHash = table.Column<int>(type: "int", nullable: false),
                    GrainTypeString = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    GrainIdExtensionString = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ServiceId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PayloadBinary = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PayloadXml = table.Column<string>(type: "xml", nullable: true),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "DATETIME2(3)", precision: 3, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "OrleansMembershipTable",
                columns: table => new
                {
                    DeploymentId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Generation = table.Column<int>(type: "int", nullable: false),
                    SiloName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HostName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProxyPort = table.Column<int>(type: "int", nullable: false),
                    SuspectTimes = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    StartTime = table.Column<DateTime>(type: "DATETIME2(3)", precision: 3, nullable: false),
                    IAmAliveTime = table.Column<DateTime>(type: "DATETIME2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrleansMembershipTable", x => new { x.DeploymentId, x.Address, x.Port, x.Generation });
                    table.ForeignKey(
                        name: "FK_OrleansMembershipTable_OrleansMembershipVersionTable_DeploymentId",
                        column: x => x.DeploymentId,
                        principalTable: "OrleansMembershipVersionTable",
                        principalColumn: "DeploymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrleansStorage_GrainIdHash",
                table: "OrleansStorage",
                column: "GrainIdHash")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrleansStorage_GrainTypeHash",
                table: "OrleansStorage",
                column: "GrainTypeHash")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrleansMembershipTable");

            migrationBuilder.DropTable(
                name: "OrleansQuery");

            migrationBuilder.DropTable(
                name: "OrleansRemindersTable");

            migrationBuilder.DropTable(
                name: "OrleansStorage");

            migrationBuilder.DropTable(
                name: "OrleansMembershipVersionTable");
        }
    }
}
