using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixdbflow6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("33d0e7b0-31f9-4f69-87e8-4e4f82e72b3d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("85b27f8b-dd5b-4821-8f39-a6288375c0db"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("1552b3d8-c5d6-446e-8a57-c7dba9d5e4f9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("5e57d970-8d9d-474a-96b6-f8c4bc49e121"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 2, 21, 25, 57, 683, DateTimeKind.Local).AddTicks(4874),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 31, 16, 33, 27, 773, DateTimeKind.Local).AddTicks(6478));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8b03b858-f03b-4346-87f5-9fe495c73bb7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e9ca3ccd-6682-49f9-8d8a-172e72731b98"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("644135c4-c137-44e8-91f5-ab02a307f942"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("362d98bf-3903-4cd1-9c75-2ac68cac3398"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ad028510-ca1a-4812-baff-e4ec876cc592"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("03c7dd10-a67a-46e1-867e-f97de477f4aa"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("32eb25dc-60b5-40bd-a40e-65d5609c5066"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("42fc60cb-ca84-42da-b552-d76aa7f735ac"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("082a0954-c169-4d5c-a0e4-e13a1494bfe5"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("46ceb2a3-51ba-44f1-8135-19369240c56e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e03826af-6e90-42fe-aecb-62184573e327"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d63d391f-82ee-4596-88d7-37fa322d8945"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9fb9518c-1c27-45c0-ae7c-b8870f66a4b3"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2954d688-145b-437f-a543-10101d90841b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("dd163674-0e2a-4265-992b-684884abc4d7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b2c64ce1-8563-44a4-9df5-d8c840cd08a2"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "DefendCapstoneProjectInformationCalendar",
                type: "text",
                nullable: false,
                defaultValue: "NotStarted");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ee7e9dfc-3602-4b8c-8ae0-7f783a1fa4b0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("39f79e71-5b60-4138-ad0f-c29069509a35"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("396aa47c-b463-4771-a1b4-83452ff6dd41"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2ed5875c-83bb-4876-a380-db219be6ee87"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "DefendCapstoneProjectInformationCalendar");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("85b27f8b-dd5b-4821-8f39-a6288375c0db"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("33d0e7b0-31f9-4f69-87e8-4e4f82e72b3d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5e57d970-8d9d-474a-96b6-f8c4bc49e121"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("1552b3d8-c5d6-446e-8a57-c7dba9d5e4f9"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 31, 16, 33, 27, 773, DateTimeKind.Local).AddTicks(6478),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 2, 21, 25, 57, 683, DateTimeKind.Local).AddTicks(4874));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e9ca3ccd-6682-49f9-8d8a-172e72731b98"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8b03b858-f03b-4346-87f5-9fe495c73bb7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("362d98bf-3903-4cd1-9c75-2ac68cac3398"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("644135c4-c137-44e8-91f5-ab02a307f942"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("03c7dd10-a67a-46e1-867e-f97de477f4aa"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ad028510-ca1a-4812-baff-e4ec876cc592"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("42fc60cb-ca84-42da-b552-d76aa7f735ac"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("32eb25dc-60b5-40bd-a40e-65d5609c5066"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("46ceb2a3-51ba-44f1-8135-19369240c56e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("082a0954-c169-4d5c-a0e4-e13a1494bfe5"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d63d391f-82ee-4596-88d7-37fa322d8945"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e03826af-6e90-42fe-aecb-62184573e327"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2954d688-145b-437f-a543-10101d90841b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9fb9518c-1c27-45c0-ae7c-b8870f66a4b3"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b2c64ce1-8563-44a4-9df5-d8c840cd08a2"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("dd163674-0e2a-4265-992b-684884abc4d7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("39f79e71-5b60-4138-ad0f-c29069509a35"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ee7e9dfc-3602-4b8c-8ae0-7f783a1fa4b0"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2ed5875c-83bb-4876-a380-db219be6ee87"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("396aa47c-b463-4771-a1b4-83452ff6dd41"));
        }
    }
}
