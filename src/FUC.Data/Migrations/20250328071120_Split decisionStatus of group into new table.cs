using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitdecisionStatusofgroupintonewtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Decision",
                table: "Group");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d84b120f-fcb0-4631-bcc6-d3ecbf851ed7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c2ad9a58-1bb9-47fd-9f65-a80fca2bff3b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e9c35349-da71-4540-ad9f-8b1fa909a3b9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("06f05091-42be-4452-8cbc-b70ad00160c9"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 28, 14, 11, 19, 516, DateTimeKind.Local).AddTicks(9786),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 25, 21, 12, 58, 153, DateTimeKind.Local).AddTicks(883));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("92399c72-6e74-458d-b7ff-b8e9488bbb6e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("040e98e3-4a0d-4575-b9d6-53fdaf5045b4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a00f917b-2915-459d-bedc-fe88464edce1"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d6cd0787-44a2-457b-a677-4e075840bfa3"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("32b41683-9d48-4b1c-be25-ac04fad70182"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("01aabced-7cf0-4e37-8bca-b3b4292d8960"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("cd124f07-2fe5-44aa-a4b8-8898a5c30844"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("6d1a531c-46d3-4fd4-a387-116d08594df1"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b254351f-f7e7-4b99-8e08-dcd47f30c65c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("27c0557c-05ed-4774-8e2a-02db45a5229b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7ec21167-5003-42ca-a7a9-67a2a7b494fb"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c1fb0ec8-e538-4c90-9144-699dadf1adc6"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3aa241d1-611f-4356-a633-53d21630127d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9192da14-c392-4b93-9270-38faae125f0f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9580b1d1-94fc-480a-a57e-e441e5abe831"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d83d5544-421c-45a5-bdaf-d3ccf5f90f0b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7d1a5b13-8602-44ac-9a2e-a618904f2c55"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("1ce1d083-d9ee-4a38-9722-038f36d853b3"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4715cb14-cd37-4115-81e3-10765b6f11e7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d80371c4-2d64-46ef-a67f-3bf9334f95bc"));

            migrationBuilder.CreateTable(
                name: "DefendCapstoneProjectDecision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Decision = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefendCapstoneProjectDecision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectDecision_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectDecision_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectDecision_GroupId",
                table: "DefendCapstoneProjectDecision",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectDecision_SupervisorId",
                table: "DefendCapstoneProjectDecision",
                column: "SupervisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefendCapstoneProjectDecision");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c2ad9a58-1bb9-47fd-9f65-a80fca2bff3b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d84b120f-fcb0-4631-bcc6-d3ecbf851ed7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("06f05091-42be-4452-8cbc-b70ad00160c9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e9c35349-da71-4540-ad9f-8b1fa909a3b9"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 25, 21, 12, 58, 153, DateTimeKind.Local).AddTicks(883),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 28, 14, 11, 19, 516, DateTimeKind.Local).AddTicks(9786));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("040e98e3-4a0d-4575-b9d6-53fdaf5045b4"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("92399c72-6e74-458d-b7ff-b8e9488bbb6e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d6cd0787-44a2-457b-a677-4e075840bfa3"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a00f917b-2915-459d-bedc-fe88464edce1"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("01aabced-7cf0-4e37-8bca-b3b4292d8960"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("32b41683-9d48-4b1c-be25-ac04fad70182"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("6d1a531c-46d3-4fd4-a387-116d08594df1"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("cd124f07-2fe5-44aa-a4b8-8898a5c30844"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("27c0557c-05ed-4774-8e2a-02db45a5229b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b254351f-f7e7-4b99-8e08-dcd47f30c65c"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c1fb0ec8-e538-4c90-9144-699dadf1adc6"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7ec21167-5003-42ca-a7a9-67a2a7b494fb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9192da14-c392-4b93-9270-38faae125f0f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3aa241d1-611f-4356-a633-53d21630127d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d83d5544-421c-45a5-bdaf-d3ccf5f90f0b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9580b1d1-94fc-480a-a57e-e441e5abe831"));

            migrationBuilder.AddColumn<string>(
                name: "Decision",
                table: "Group",
                type: "text",
                nullable: false,
                defaultValue: "Undefined");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("1ce1d083-d9ee-4a38-9722-038f36d853b3"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7d1a5b13-8602-44ac-9a2e-a618904f2c55"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d80371c4-2d64-46ef-a67f-3bf9334f95bc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4715cb14-cd37-4115-81e3-10765b6f11e7"));
        }
    }
}
