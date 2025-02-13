using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations;

/// <inheritdoc />
public partial class AddSupervisorGroupManagementTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SupervisorGroupAssignment",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                SupervisorId = table.Column<string>(type: "text", nullable: false),
                Supervisor2Id = table.Column<string>(type: "text", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SupervisorGroupAssignment", x => x.Id);
                table.ForeignKey(
                    name: "FK_SupervisorGroupAssignment_Group_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Group",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SupervisorGroupAssignment_Supervisor_Supervisor2Id",
                    column: x => x.Supervisor2Id,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_SupervisorGroupAssignment_Supervisor_SupervisorId",
                    column: x => x.SupervisorId,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SupervisorGroupAssignment_GroupId",
            table: "SupervisorGroupAssignment",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_SupervisorGroupAssignment_Supervisor2Id",
            table: "SupervisorGroupAssignment",
            column: "Supervisor2Id");

        migrationBuilder.CreateIndex(
            name: "IX_SupervisorGroupAssignment_SupervisorId",
            table: "SupervisorGroupAssignment",
            column: "SupervisorId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SupervisorGroupAssignment");
    }
}
