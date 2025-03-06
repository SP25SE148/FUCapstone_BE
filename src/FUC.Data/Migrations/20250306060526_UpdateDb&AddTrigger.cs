using System;
using FUC.Data.Constants;
using FUC.Data.Enums;
using MassTransit.SqlTransport.Topology;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbAddTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
            CREATE OR REPLACE FUNCTION check_topic_status_before_update()
            RETURNS TRIGGER AS $$
            BEGIN
                IF NEW.""IsAssignedToGroup"" = true AND NEW.""Status"" <> '{TopicStatus.Approved.ToString()}' THEN
                    RAISE EXCEPTION 'Cannot assign topic if status is not {TopicStatus.Approved.ToString()}';
                END IF;
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;

            CREATE TRIGGER enforce_topic_status
            BEFORE UPDATE ON ""{TableNames.Topic}""
            FOR EACH ROW EXECUTE FUNCTION check_topic_status_before_update();
        ");
            
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9b03ced1-d34d-4e97-95ed-77f157fb3560"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("50be2d60-3ef7-4089-bfd5-67dddeb32e04"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b8fe0d56-5306-4e5c-9afd-0f1f98118822"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("73495ea7-d2f5-4668-99c4-631ec08e71ab"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 6, 6, 5, 26, 27, DateTimeKind.Utc).AddTicks(5553),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 5, 8, 3, 14, 694, DateTimeKind.Utc).AddTicks(2692));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7da6db51-840d-4333-add4-567ac94d15db"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("62cbab6a-16b4-4a65-8b41-f90afdeb9be0"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("fd355f8c-28a3-47c7-974d-66064d4a8721"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("1ac63826-2126-4f6d-86ab-7867e1efe4cb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a0f92b5c-f6e9-4d9c-b616-fb86f6229b9f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d77b5d43-ee03-405d-961b-c20329229ecc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SupervisorGroupAssignment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0f43c2ea-9d7a-4dda-ae0e-5585a7950a90"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f596ce5b-0dfd-4fb7-bdf0-da5898b24b71"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4aa61afa-1bcf-44be-b8f0-288cf3770152"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a2e55a07-3b59-4a80-b080-2a99452c9b17"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("91602363-dd29-46b0-a1cb-30121466964b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f371ba63-3dc2-416d-aca2-33f3d4f8a911"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("735b0063-45a3-45af-859a-0e62552fba27"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("35ac8021-ee1d-4c4b-a833-6565ba505c82"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c117f1ed-bd15-46b7-a83d-c32ef53c3167"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("07bc7cb0-df0d-4cb7-8d27-6881ee29932e"));

            migrationBuilder.CreateTable(
                name: "ProjectProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MeetingDate = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProgress_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectProgress_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProgressWeek",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectProgressId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    MeetingLocation = table.Column<string>(type: "text", nullable: true),
                    MeetingContent = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProgressWeek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProgressWeek_ProjectProgress_ProjectProgressId",
                        column: x => x.ProjectProgressId,
                        principalTable: "ProjectProgress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FucTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyTask = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    AssigneeId = table.Column<string>(type: "text", nullable: false),
                    ReporterId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "ToDo"),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Attachments = table.Column<string>(type: "text", nullable: true),
                    ProjectProgressWeekId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FucTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FucTask_ProjectProgressWeek_ProjectProgressWeekId",
                        column: x => x.ProjectProgressWeekId,
                        principalTable: "ProjectProgressWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FucTask_Student_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FucTask_Student_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyEvaluation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    ProjectProgressWeekId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContributionPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_WeeklyEvaluation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyEvaluation_ProjectProgressWeek_ProjectProgressWeekId",
                        column: x => x.ProjectProgressWeekId,
                        principalTable: "ProjectProgressWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeeklyEvaluation_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeeklyEvaluation_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FucTaskHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FucTaskHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FucTaskHistory_FucTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "FucTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FucTask_AssigneeId",
                table: "FucTask",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_FucTask_ProjectProgressWeekId",
                table: "FucTask",
                column: "ProjectProgressWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_FucTask_ReporterId",
                table: "FucTask",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_FucTaskHistory_TaskId",
                table: "FucTaskHistory",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProgress_GroupId",
                table: "ProjectProgress",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProgress_SupervisorId",
                table: "ProjectProgress",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProgressWeek_ProjectProgressId",
                table: "ProjectProgressWeek",
                column: "ProjectProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyEvaluation_ProjectProgressWeekId",
                table: "WeeklyEvaluation",
                column: "ProjectProgressWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyEvaluation_StudentId",
                table: "WeeklyEvaluation",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyEvaluation_SupervisorId",
                table: "WeeklyEvaluation",
                column: "SupervisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS enforce_topic_status ON \"Topic\";");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS check_topic_status_before_update;");
            
            migrationBuilder.DropTable(
                name: "FucTaskHistory");

            migrationBuilder.DropTable(
                name: "WeeklyEvaluation");

            migrationBuilder.DropTable(
                name: "FucTask");

            migrationBuilder.DropTable(
                name: "ProjectProgressWeek");

            migrationBuilder.DropTable(
                name: "ProjectProgress");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("50be2d60-3ef7-4089-bfd5-67dddeb32e04"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9b03ced1-d34d-4e97-95ed-77f157fb3560"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("73495ea7-d2f5-4668-99c4-631ec08e71ab"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b8fe0d56-5306-4e5c-9afd-0f1f98118822"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 5, 8, 3, 14, 694, DateTimeKind.Utc).AddTicks(2692),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 6, 6, 5, 26, 27, DateTimeKind.Utc).AddTicks(5553));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("62cbab6a-16b4-4a65-8b41-f90afdeb9be0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7da6db51-840d-4333-add4-567ac94d15db"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("1ac63826-2126-4f6d-86ab-7867e1efe4cb"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("fd355f8c-28a3-47c7-974d-66064d4a8721"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d77b5d43-ee03-405d-961b-c20329229ecc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a0f92b5c-f6e9-4d9c-b616-fb86f6229b9f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SupervisorGroupAssignment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f596ce5b-0dfd-4fb7-bdf0-da5898b24b71"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0f43c2ea-9d7a-4dda-ae0e-5585a7950a90"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a2e55a07-3b59-4a80-b080-2a99452c9b17"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4aa61afa-1bcf-44be-b8f0-288cf3770152"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f371ba63-3dc2-416d-aca2-33f3d4f8a911"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("91602363-dd29-46b0-a1cb-30121466964b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("35ac8021-ee1d-4c4b-a833-6565ba505c82"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("735b0063-45a3-45af-859a-0e62552fba27"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("07bc7cb0-df0d-4cb7-8d27-6881ee29932e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c117f1ed-bd15-46b7-a83d-c32ef53c3167"));
        }
    }
}
