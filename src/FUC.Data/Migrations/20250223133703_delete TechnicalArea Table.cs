using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class deleteTechnicalAreaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentExpertise");

            migrationBuilder.DropTable(
                name: "TechnicalArea");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GroupMember",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "GroupMember",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "GroupMember",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "GroupMember",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GroupMember");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "GroupMember");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "GroupMember");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "GroupMember");

            migrationBuilder.CreateTable(
                name: "TechnicalArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentExpertise",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    TechnicalAreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentExpertise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentExpertise_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentExpertise_TechnicalArea_TechnicalAreaId",
                        column: x => x.TechnicalAreaId,
                        principalTable: "TechnicalArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentExpertise_StudentId",
                table: "StudentExpertise",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExpertise_TechnicalAreaId",
                table: "StudentExpertise",
                column: "TechnicalAreaId");
        }
    }
}
