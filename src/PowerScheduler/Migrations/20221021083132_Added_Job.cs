using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerScheduler.Migrations
{
    public partial class Added_Job : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PowerSchedulerJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Labels = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    JobPriority = table.Column<int>(type: "int", nullable: false),
                    JobArgs = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAbandoned = table.Column<bool>(type: "bit", nullable: false),
                    JobType = table.Column<int>(type: "int", nullable: false),
                    ExecutionMode = table.Column<int>(type: "int", nullable: false),
                    ProcessorInfo = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TimeExpression = table.Column<int>(type: "int", nullable: false),
                    TimeExpressionValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BeginTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MaxTryCount = table.Column<int>(type: "int", nullable: false),
                    TimeoutSecond = table.Column<int>(type: "int", nullable: false),
                    NextTriggerTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastTriggerTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MisfireStrategy = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerSchedulerJob", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerSchedulerTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskArgs = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TaskRunStatus = table.Column<int>(type: "int", nullable: false),
                    ExpectedTriggerTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActualTriggerTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FinishedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    WorkerHost = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Result = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TryCount = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerSchedulerTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerSchedulerTask_PowerSchedulerJob_JobId",
                        column: x => x.JobId,
                        principalTable: "PowerSchedulerJob",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PowerSchedulerTask_JobId",
                table: "PowerSchedulerTask",
                column: "JobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PowerSchedulerTask");

            migrationBuilder.DropTable(
                name: "PowerSchedulerJob");
        }
    }
}
