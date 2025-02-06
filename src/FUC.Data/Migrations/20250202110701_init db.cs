using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations;

/// <inheritdoc />
public partial class initdb : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Campus",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Address = table.Column<string>(type: "text", nullable: false),
                Phone = table.Column<string>(type: "text", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Campus", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "MajorGroup",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MajorGroup", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Semester",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Semester", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Major",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                MajorGroupId = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Major", x => x.Id);
                table.ForeignKey(
                    name: "FK_Major_MajorGroup_MajorGroupId",
                    column: x => x.MajorGroupId,
                    principalTable: "MajorGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Capstone",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                MajorId = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                MinMember = table.Column<int>(type: "integer", nullable: false),
                MaxMember = table.Column<int>(type: "integer", nullable: false),
                ReviewCount = table.Column<int>(type: "integer", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Capstone", x => x.Id);
                table.ForeignKey(
                    name: "FK_Capstone_Major_MajorId",
                    column: x => x.MajorId,
                    principalTable: "Major",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Group",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SemesterId = table.Column<string>(type: "text", nullable: false),
                MajorId = table.Column<string>(type: "text", nullable: false),
                CampusId = table.Column<string>(type: "text", nullable: false),
                TopicId = table.Column<Guid>(type: "uuid", nullable: true),
                CapstoneId = table.Column<string>(type: "text", nullable: false),
                TopicCode = table.Column<string>(type: "text", nullable: true),
                GroupCode = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Group", x => x.Id);
                table.ForeignKey(
                    name: "FK_Group_Campus_CampusId",
                    column: x => x.CampusId,
                    principalTable: "Campus",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Group_Capstone_CapstoneId",
                    column: x => x.CapstoneId,
                    principalTable: "Capstone",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Group_Major_MajorId",
                    column: x => x.MajorId,
                    principalTable: "Major",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Group_Semester_SemesterId",
                    column: x => x.SemesterId,
                    principalTable: "Semester",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Student",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                MajorId = table.Column<string>(type: "text", nullable: false),
                CapstoneId = table.Column<string>(type: "text", nullable: false),
                CampusId = table.Column<string>(type: "text", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                IsEligible = table.Column<bool>(type: "boolean", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Student", x => x.Id);
                table.ForeignKey(
                    name: "FK_Student_Campus_CampusId",
                    column: x => x.CampusId,
                    principalTable: "Campus",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Student_Capstone_CampusId",
                    column: x => x.CampusId,
                    principalTable: "Capstone",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Student_Major_MajorId",
                    column: x => x.MajorId,
                    principalTable: "Major",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "GroupMember",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                StudentId = table.Column<string>(type: "text", nullable: false),
                IsLeader = table.Column<bool>(type: "boolean", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GroupMember", x => x.Id);
                table.ForeignKey(
                    name: "FK_GroupMember_Group_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Group",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_GroupMember_Student_StudentId",
                    column: x => x.StudentId,
                    principalTable: "Student",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Capstone_MajorId",
            table: "Capstone",
            column: "MajorId");

        migrationBuilder.CreateIndex(
            name: "IX_Group_CampusId",
            table: "Group",
            column: "CampusId");

        migrationBuilder.CreateIndex(
            name: "IX_Group_CapstoneId",
            table: "Group",
            column: "CapstoneId");

        migrationBuilder.CreateIndex(
            name: "IX_Group_MajorId",
            table: "Group",
            column: "MajorId");

        migrationBuilder.CreateIndex(
            name: "IX_Group_SemesterId",
            table: "Group",
            column: "SemesterId");

        migrationBuilder.CreateIndex(
            name: "IX_GroupMember_GroupId",
            table: "GroupMember",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_GroupMember_StudentId",
            table: "GroupMember",
            column: "StudentId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Major_MajorGroupId",
            table: "Major",
            column: "MajorGroupId");

        migrationBuilder.CreateIndex(
            name: "IX_Student_CampusId",
            table: "Student",
            column: "CampusId");

        migrationBuilder.CreateIndex(
            name: "IX_Student_MajorId",
            table: "Student",
            column: "MajorId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GroupMember");

        migrationBuilder.DropTable(
            name: "Group");

        migrationBuilder.DropTable(
            name: "Student");

        migrationBuilder.DropTable(
            name: "Semester");

        migrationBuilder.DropTable(
            name: "Campus");

        migrationBuilder.DropTable(
            name: "Capstone");

        migrationBuilder.DropTable(
            name: "Major");

        migrationBuilder.DropTable(
            name: "MajorGroup");
    }
}
