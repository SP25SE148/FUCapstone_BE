using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class addtablesflow3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("73495ea7-d2f5-4668-99c4-631ec08e71ab"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8b3e797b-8cce-4f1a-9a2a-8561087cf66f"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 5, 8, 3, 14, 694, DateTimeKind.Utc).AddTicks(2692),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 3, 14, 44, 14, 713, DateTimeKind.Utc).AddTicks(8012));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("62cbab6a-16b4-4a65-8b41-f90afdeb9be0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ad311d7f-dc2e-4c2e-9622-7c3b41abb592"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("1ac63826-2126-4f6d-86ab-7867e1efe4cb"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("73e1d637-7862-4e69-b48e-c2dc068ed86e"));

            migrationBuilder.AddColumn<bool>(
                name: "IsAssignedToGroup",
                table: "Topic",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d77b5d43-ee03-405d-961b-c20329229ecc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("5ab414f5-5277-4056-873b-80300631fbe4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SupervisorGroupAssignment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f596ce5b-0dfd-4fb7-bdf0-da5898b24b71"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("1d919744-1378-4816-ab4b-a741dd05ccbb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a2e55a07-3b59-4a80-b080-2a99452c9b17"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("fd71a591-3c3c-491f-81d2-345fb9b7a273"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f371ba63-3dc2-416d-aca2-33f3d4f8a911"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e39ca355-abfe-41a0-8052-283d3f6c7727"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("35ac8021-ee1d-4c4b-a833-6565ba505c82"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("42002c2c-35e8-4ace-a420-ba78c6d5b5bf"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("07bc7cb0-df0d-4cb7-8d27-6881ee29932e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("87db72ec-a498-4fe4-9eda-d5fc5cbfc749"));

            migrationBuilder.CreateTable(
                name: "TopicRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("50be2d60-3ef7-4089-bfd5-67dddeb32e04")),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "UnderReview"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicRequest_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopicRequest_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopicRequest_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicRequest_GroupId",
                table: "TopicRequest",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicRequest_SupervisorId",
                table: "TopicRequest",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicRequest_TopicId",
                table: "TopicRequest",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicRequest");

            migrationBuilder.DropColumn(
                name: "IsAssignedToGroup",
                table: "Topic");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8b3e797b-8cce-4f1a-9a2a-8561087cf66f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("73495ea7-d2f5-4668-99c4-631ec08e71ab"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 3, 14, 44, 14, 713, DateTimeKind.Utc).AddTicks(8012),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 5, 8, 3, 14, 694, DateTimeKind.Utc).AddTicks(2692));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ad311d7f-dc2e-4c2e-9622-7c3b41abb592"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("62cbab6a-16b4-4a65-8b41-f90afdeb9be0"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("73e1d637-7862-4e69-b48e-c2dc068ed86e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("1ac63826-2126-4f6d-86ab-7867e1efe4cb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5ab414f5-5277-4056-873b-80300631fbe4"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d77b5d43-ee03-405d-961b-c20329229ecc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SupervisorGroupAssignment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("1d919744-1378-4816-ab4b-a741dd05ccbb"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f596ce5b-0dfd-4fb7-bdf0-da5898b24b71"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("fd71a591-3c3c-491f-81d2-345fb9b7a273"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a2e55a07-3b59-4a80-b080-2a99452c9b17"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e39ca355-abfe-41a0-8052-283d3f6c7727"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f371ba63-3dc2-416d-aca2-33f3d4f8a911"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("42002c2c-35e8-4ace-a420-ba78c6d5b5bf"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("35ac8021-ee1d-4c4b-a833-6565ba505c82"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("87db72ec-a498-4fe4-9eda-d5fc5cbfc749"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("07bc7cb0-df0d-4cb7-8d27-6881ee29932e"));
        }
    }
}
