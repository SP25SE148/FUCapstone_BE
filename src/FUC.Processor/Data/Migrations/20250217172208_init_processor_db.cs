using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Processor.Data.Migrations
{
    /// <inheritdoc />
    public partial class init_processor_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ReminderType = table.Column<string>(type: "text", nullable: false),
                    RemindFor = table.Column<string>(type: "text", nullable: false),
                    RemindDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reminders");
        }
    }
}
