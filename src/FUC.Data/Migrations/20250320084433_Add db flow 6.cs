using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Adddbflow6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f97df866-6677-4545-ac3c-daa5ab2109c4"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("72e87ba8-b77a-416c-bb5f-ca2b4b32c810"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ba833037-003c-4fe8-b8a3-56b876afdfea"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("5b6be931-68ff-4042-9184-bfa9963949dc"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 20, 15, 44, 32, 448, DateTimeKind.Local).AddTicks(6384),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 19, 18, 25, 20, 506, DateTimeKind.Local).AddTicks(9872));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("824d8d6d-d86c-4da3-9974-a1aebad367cb"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("44964614-eea6-4f11-bfcc-7b43bcdc63e5"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e5cfe773-9f9e-4d25-b862-e6e0825264e9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("fd3b2cd6-457b-4f0a-acea-5c16351ca372"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c9dc72d6-cca3-48b3-a53c-42cd45858c3d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7b9a7dba-1a6f-414a-96d5-11fdc87c718c"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("af0007e7-e612-4d26-b0e3-05237a1d79b0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0dc5cb0f-730c-409c-9815-224f9046b9b8"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("6c9f0142-312d-4a00-8018-e9b28c9bd22c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("dfb9d05f-4b61-4d07-9926-1924edcc3995"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("70cae8e5-b195-4927-8fe1-525c09344ee6"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4400af60-d648-4500-aa06-3aa792c9c81d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("840056db-7125-4708-84cc-ebf453e79fdd"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("dac99c1a-9442-4be6-a3e6-59175fa9fba5"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("63cccab4-63f4-4555-bc88-fbdddd13f4c8"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f8799781-e4cb-428d-834b-dc4f61e941bd"));

            migrationBuilder.AddColumn<string>(
                name: "Decision",
                table: "Group",
                type: "text",
                nullable: false,
                defaultValue: "Attempt1");

            migrationBuilder.AddColumn<bool>(
                name: "IsReDefendCapstoneProject",
                table: "Group",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8c518db5-9c3c-4180-b1f2-c3d50a3941a4"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("267b893a-8879-45e7-b385-1bbf5a3cc391"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c1d08c11-79da-4f15-934d-4d12d2a88bbc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("cdb929f5-38fe-4d55-b9c1-f3529249d292"));

            migrationBuilder.CreateTable(
                name: "DefendCapstoneProjectInformationCalendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampusId = table.Column<string>(type: "text", nullable: false),
                    SemesterId = table.Column<string>(type: "text", nullable: false),
                    DefendAttempt = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefendCapstoneProjectInformationCalendar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectInformationCalendar_Campus_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectInformationCalendar_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectInformationCalendar_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefendCapstoneProjectCouncilMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefendCapstoneProjectInformationCalendarId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    IsPresident = table.Column<bool>(type: "boolean", nullable: false),
                    IsSecretary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefendCapstoneProjectCouncilMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectCouncilMember_DefendCapstoneProjectInf~",
                        column: x => x.DefendCapstoneProjectInformationCalendarId,
                        principalTable: "DefendCapstoneProjectInformationCalendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectCouncilMember_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectCouncilMember_DefendCapstoneProjectInf~",
                table: "DefendCapstoneProjectCouncilMember",
                column: "DefendCapstoneProjectInformationCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectCouncilMember_SupervisorId",
                table: "DefendCapstoneProjectCouncilMember",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_CampusId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_SemesterId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_TopicId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefendCapstoneProjectCouncilMember");

            migrationBuilder.DropTable(
                name: "DefendCapstoneProjectInformationCalendar");

            migrationBuilder.DropColumn(
                name: "Decision",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "IsReDefendCapstoneProject",
                table: "Group");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("72e87ba8-b77a-416c-bb5f-ca2b4b32c810"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f97df866-6677-4545-ac3c-daa5ab2109c4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5b6be931-68ff-4042-9184-bfa9963949dc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ba833037-003c-4fe8-b8a3-56b876afdfea"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 19, 18, 25, 20, 506, DateTimeKind.Local).AddTicks(9872),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 20, 15, 44, 32, 448, DateTimeKind.Local).AddTicks(6384));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("44964614-eea6-4f11-bfcc-7b43bcdc63e5"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("824d8d6d-d86c-4da3-9974-a1aebad367cb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("fd3b2cd6-457b-4f0a-acea-5c16351ca372"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e5cfe773-9f9e-4d25-b862-e6e0825264e9"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7b9a7dba-1a6f-414a-96d5-11fdc87c718c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c9dc72d6-cca3-48b3-a53c-42cd45858c3d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0dc5cb0f-730c-409c-9815-224f9046b9b8"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("af0007e7-e612-4d26-b0e3-05237a1d79b0"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("dfb9d05f-4b61-4d07-9926-1924edcc3995"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("6c9f0142-312d-4a00-8018-e9b28c9bd22c"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4400af60-d648-4500-aa06-3aa792c9c81d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("70cae8e5-b195-4927-8fe1-525c09344ee6"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("dac99c1a-9442-4be6-a3e6-59175fa9fba5"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("840056db-7125-4708-84cc-ebf453e79fdd"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f8799781-e4cb-428d-834b-dc4f61e941bd"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("63cccab4-63f4-4555-bc88-fbdddd13f4c8"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("267b893a-8879-45e7-b385-1bbf5a3cc391"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8c518db5-9c3c-4180-b1f2-c3d50a3941a4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("cdb929f5-38fe-4d55-b9c1-f3529249d292"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c1d08c11-79da-4f15-934d-4d12d2a88bbc"));
        }
    }
}
