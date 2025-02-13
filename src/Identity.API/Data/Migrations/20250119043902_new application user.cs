using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.API.Data.Migrations;

/// <inheritdoc />
public partial class newapplicationuser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "MajorId",
            table: "AspNetUsers",
            type: "text",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "CampusId",
            table: "AspNetUsers",
            type: "text",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AddColumn<string>(
            name: "CapstoneId",
            table: "AspNetUsers",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "UserCode",
            table: "AspNetUsers",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CapstoneId",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "UserCode",
            table: "AspNetUsers");

        migrationBuilder.AlterColumn<int>(
            name: "MajorId",
            table: "AspNetUsers",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AlterColumn<int>(
            name: "CampusId",
            table: "AspNetUsers",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");
    }
}
