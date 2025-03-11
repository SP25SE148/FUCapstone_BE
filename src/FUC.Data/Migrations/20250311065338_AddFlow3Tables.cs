using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFlow3Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3f6809ea-23dc-4022-b2bd-d2bbe5851e7a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("77ed9039-f30e-42ba-a4fb-935e20374153"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c2db297c-41ea-4a64-80ea-d4ef2e5462a9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("20615138-df5e-4ae0-9f1e-277715d866a5"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 11, 6, 53, 37, 551, DateTimeKind.Utc).AddTicks(5849),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 10, 11, 48, 46, 168, DateTimeKind.Utc).AddTicks(9562));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4b3c396c-3a35-40d1-93d7-ec3d9b28c77d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("236e31b3-1a02-438d-b7df-3e78955c7b67"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("83b4093b-b3aa-47df-8885-d464d54f0b25"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9fc88187-7c47-43c0-a2a3-b6ad06ec0018"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("626342f1-59cb-4bb3-b90f-9200a5861b4b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("59601dde-99e0-49c8-bd84-123a7084e78a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("cb536904-3bcf-43a5-90d3-003a8bec71d0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("fe2ec122-afe5-42d7-a194-b04de7a555ac"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e8544c3f-89bd-4eea-96dd-80304b761b56"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("fd2b0189-b502-448e-b38b-c24f4d7e243f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0a7dc7a0-9c07-42c9-aaa6-9893be6bc1be"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("58c4edf0-ad9c-4c5a-99a9-6327c0d5e53b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("482b88c7-9113-4df1-a8ed-137ab1585837"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("59ce53fb-d946-4bb3-89c8-bab2e211cdad"));

            migrationBuilder.CreateTable(
                name: "ReviewCalendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("1b947fdf-6588-4449-9ffd-08013e867e11")),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    MajorId = table.Column<string>(type: "text", nullable: false),
                    CampusId = table.Column<string>(type: "text", nullable: false),
                    SemesterId = table.Column<string>(type: "text", nullable: false),
                    Attempt = table.Column<int>(type: "integer", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    Room = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_ReviewCalendar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Campus_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Major_MajorId",
                        column: x => x.MajorId,
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("a09bd06a-198e-42ad-b274-6fef55d9aadf")),
                    CapstoneId = table.Column<string>(type: "text", nullable: false),
                    Attempt = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Requirement = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewCriteria_Capstone_CapstoneId",
                        column: x => x.CapstoneId,
                        principalTable: "Capstone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviewer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("e5a9f5d5-9a70-45c8-9c57-5100d1a5742a")),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    ReviewCalenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Suggestion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviewer_ReviewCalendar_ReviewCalenderId",
                        column: x => x.ReviewCalenderId,
                        principalTable: "ReviewCalendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviewer_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_CampusId",
                table: "ReviewCalendar",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_GroupId",
                table: "ReviewCalendar",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_MajorId",
                table: "ReviewCalendar",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_SemesterId",
                table: "ReviewCalendar",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_TopicId",
                table: "ReviewCalendar",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCriteria_CapstoneId",
                table: "ReviewCriteria",
                column: "CapstoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewer_ReviewCalenderId",
                table: "Reviewer",
                column: "ReviewCalenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewer_SupervisorId",
                table: "Reviewer",
                column: "SupervisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewCriteria");

            migrationBuilder.DropTable(
                name: "Reviewer");

            migrationBuilder.DropTable(
                name: "ReviewCalendar");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("77ed9039-f30e-42ba-a4fb-935e20374153"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3f6809ea-23dc-4022-b2bd-d2bbe5851e7a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("20615138-df5e-4ae0-9f1e-277715d866a5"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c2db297c-41ea-4a64-80ea-d4ef2e5462a9"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 10, 11, 48, 46, 168, DateTimeKind.Utc).AddTicks(9562),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 11, 6, 53, 37, 551, DateTimeKind.Utc).AddTicks(5849));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("236e31b3-1a02-438d-b7df-3e78955c7b67"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4b3c396c-3a35-40d1-93d7-ec3d9b28c77d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9fc88187-7c47-43c0-a2a3-b6ad06ec0018"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("83b4093b-b3aa-47df-8885-d464d54f0b25"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("59601dde-99e0-49c8-bd84-123a7084e78a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("626342f1-59cb-4bb3-b90f-9200a5861b4b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("fe2ec122-afe5-42d7-a194-b04de7a555ac"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("cb536904-3bcf-43a5-90d3-003a8bec71d0"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("fd2b0189-b502-448e-b38b-c24f4d7e243f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e8544c3f-89bd-4eea-96dd-80304b761b56"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("58c4edf0-ad9c-4c5a-99a9-6327c0d5e53b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0a7dc7a0-9c07-42c9-aaa6-9893be6bc1be"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("59ce53fb-d946-4bb3-89c8-bab2e211cdad"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("482b88c7-9113-4df1-a8ed-137ab1585837"));
        }
    }
}
