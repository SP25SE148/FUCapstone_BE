using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations;

/// <inheritdoc />
public partial class db_v003 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Student_Capstone_CampusId",
            table: "Student");

        migrationBuilder.CreateIndex(
            name: "IX_Student_CapstoneId",
            table: "Student",
            column: "CapstoneId");

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

        migrationBuilder.DropIndex(
            name: "IX_Student_CapstoneId",
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
