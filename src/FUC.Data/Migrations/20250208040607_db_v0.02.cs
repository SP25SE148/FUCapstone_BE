using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class db_v002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Capstone_CampusId",
                table: "Student");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Student",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Supervisor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    MajorId = table.Column<string>(type: "text", nullable: false),
                    CampusId = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supervisor_Campus_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Supervisor_Major_MajorId",
                        column: x => x.MajorId,
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_CapstoneId",
                table: "Student",
                column: "CapstoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_CampusId",
                table: "Supervisor",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_MajorId",
                table: "Supervisor",
                column: "MajorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Capstone_CapstoneId",
                table: "Student",
                column: "CapstoneId",
                principalTable: "Capstone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Capstone_CapstoneId",
                table: "Student");

            migrationBuilder.DropTable(
                name: "Supervisor");

            migrationBuilder.DropIndex(
                name: "IX_Student_CapstoneId",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Student");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Capstone_CampusId",
                table: "Student",
                column: "CampusId",
                principalTable: "Capstone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
