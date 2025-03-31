using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class fix_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("85b27f8b-dd5b-4821-8f39-a6288375c0db"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c54fd789-3f60-4c29-a41e-d91e08255b20"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5e57d970-8d9d-474a-96b6-f8c4bc49e121"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("22d24b58-b77b-4799-8f39-a2bf39ec33f0"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 31, 16, 33, 27, 773, DateTimeKind.Local).AddTicks(6478),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 30, 17, 4, 15, 117, DateTimeKind.Local).AddTicks(7177));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e9ca3ccd-6682-49f9-8d8a-172e72731b98"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("02ef524a-879a-4a2a-bedf-39a7eba8f05d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("362d98bf-3903-4cd1-9c75-2ac68cac3398"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c31b579e-75a0-4586-9ee7-59a6915426a3"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("03c7dd10-a67a-46e1-867e-f97de477f4aa"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3a55c7d6-db18-460a-a857-94b085ee09c4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("42fc60cb-ca84-42da-b552-d76aa7f735ac"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3e85a74a-cc46-416e-89b7-566d4bb9b502"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("46ceb2a3-51ba-44f1-8135-19369240c56e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2ebc63c8-d92c-45f7-acc8-6b8d257bcd6a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d63d391f-82ee-4596-88d7-37fa322d8945"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8ec6d2fc-816b-4c1d-b01d-373d44efdd3d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2954d688-145b-437f-a543-10101d90841b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("324e6295-d8f1-4b13-a1b6-112df04281e7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b2c64ce1-8563-44a4-9df5-d8c840cd08a2"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d5d3f7ad-45ba-4deb-a267-263014b3fcdc"));

            migrationBuilder.AddColumn<string>(
                name: "CapstoneId",
                table: "DefendCapstoneProjectInformationCalendar",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("39f79e71-5b60-4138-ad0f-c29069509a35"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d8628df1-3757-4665-b70f-ac5e3c0a9e58"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2ed5875c-83bb-4876-a380-db219be6ee87"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c980796b-2280-42b6-a4b4-bcc1b5f3aea9"));

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_CapstoneId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "CapstoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_DefendCapstoneProjectInformationCalendar_Capstone_CapstoneId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "CapstoneId",
                principalTable: "Capstone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefendCapstoneProjectInformationCalendar_Capstone_CapstoneId",
                table: "DefendCapstoneProjectInformationCalendar");

            migrationBuilder.DropIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_CapstoneId",
                table: "DefendCapstoneProjectInformationCalendar");

            migrationBuilder.DropColumn(
                name: "CapstoneId",
                table: "DefendCapstoneProjectInformationCalendar");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c54fd789-3f60-4c29-a41e-d91e08255b20"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("85b27f8b-dd5b-4821-8f39-a6288375c0db"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("22d24b58-b77b-4799-8f39-a2bf39ec33f0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("5e57d970-8d9d-474a-96b6-f8c4bc49e121"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 30, 17, 4, 15, 117, DateTimeKind.Local).AddTicks(7177),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 31, 16, 33, 27, 773, DateTimeKind.Local).AddTicks(6478));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("02ef524a-879a-4a2a-bedf-39a7eba8f05d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e9ca3ccd-6682-49f9-8d8a-172e72731b98"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c31b579e-75a0-4586-9ee7-59a6915426a3"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("362d98bf-3903-4cd1-9c75-2ac68cac3398"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3a55c7d6-db18-460a-a857-94b085ee09c4"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("03c7dd10-a67a-46e1-867e-f97de477f4aa"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3e85a74a-cc46-416e-89b7-566d4bb9b502"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("42fc60cb-ca84-42da-b552-d76aa7f735ac"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2ebc63c8-d92c-45f7-acc8-6b8d257bcd6a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("46ceb2a3-51ba-44f1-8135-19369240c56e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8ec6d2fc-816b-4c1d-b01d-373d44efdd3d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d63d391f-82ee-4596-88d7-37fa322d8945"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("324e6295-d8f1-4b13-a1b6-112df04281e7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2954d688-145b-437f-a543-10101d90841b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d5d3f7ad-45ba-4deb-a267-263014b3fcdc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b2c64ce1-8563-44a4-9df5-d8c840cd08a2"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d8628df1-3757-4665-b70f-ac5e3c0a9e58"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("39f79e71-5b60-4138-ad0f-c29069509a35"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c980796b-2280-42b6-a4b4-bcc1b5f3aea9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2ed5875c-83bb-4876-a380-db219be6ee87"));
        }
    }
}
