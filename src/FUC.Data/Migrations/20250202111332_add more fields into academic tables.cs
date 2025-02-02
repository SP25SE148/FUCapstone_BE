using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class addmorefieldsintoacademictables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Semester",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Semester",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "MajorGroup",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MajorGroup",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "MajorGroup",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "MajorGroup",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Major",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Major",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Major",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Major",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Group",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Group",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Capstone",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Capstone",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Capstone",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Capstone",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Campus",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Campus",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Campus",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Campus",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MajorGroup");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MajorGroup");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MajorGroup");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MajorGroup");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Major");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Major");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Major");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Major");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Capstone");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Capstone");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Capstone");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Capstone");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Campus");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Campus");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Campus");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Campus");
        }
    }
}
