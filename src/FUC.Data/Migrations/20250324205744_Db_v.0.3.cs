using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Db_v03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2d14d263-e812-4be8-8345-659bc467e3bc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ca886446-11d4-4ef5-9b9b-f91d01bf8b01"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("41cfb63c-8fc8-441b-88b5-e873f65cdb17"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8ce58f2c-333b-4594-b2a9-df04802ae35b"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 25, 3, 57, 44, 510, DateTimeKind.Local).AddTicks(1049),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 23, 16, 32, 49, 276, DateTimeKind.Local).AddTicks(6010));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("aded47bd-9211-4bfb-894f-bad525b29204"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4f7ba5a5-f018-4927-897c-b44a4cb52ebd"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("298feeb7-da48-463f-bbed-1cb3bf540d00"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("65d61e8d-37cc-44af-a1ab-b17f2915a30f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b62fb4e3-f19f-47d7-9f33-b51ab290617d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d5023726-1520-4aa9-ab8a-110583761046"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("69ffd5bb-76f2-4385-83c1-62ed341615e9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0a6e1b52-9892-4564-8ff9-910ea6328125"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9aa17922-41e1-46ed-99a8-ac0b99c440fc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3d657f98-8360-45ff-b9ec-4505ecbec6dc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8be6515f-506a-427c-8b18-8f163456f67b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3b2ed125-f3ee-4c1d-b15b-f197974e1d01"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3fc8d992-b4c1-4c28-b85f-dca5651b1261"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a3612480-c12b-47cf-94bb-26bf3430654d"));

            migrationBuilder.AlterColumn<string>(
                name: "Decision",
                table: "Group",
                type: "text",
                nullable: false,
                defaultValue: "Undefined",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Attempt1");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("485a9aa9-6e5a-420c-b221-d4b1494be255"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f59cc82b-12d5-4e45-b2f9-c76f9718b00f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("24ba4afe-b27c-4941-9fe6-ee4e3d1ed1c9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("bc577149-2b4a-4842-acfc-b66932a6ec86"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("111f48a9-796f-4a27-b831-6ba28034a01f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("76ed6be6-cadb-46d2-a573-375d660c8241"));

            migrationBuilder.AddCheckConstraint(
                name: "CK_DefendCapstoneProjectCouncilMembers_IsPresident_IsSecretary",
                table: "DefendCapstoneProjectCouncilMember",
                sql: "NOT (IsPresident = 1 AND IsSecretary = 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DefendCapstoneProjectCouncilMembers_IsPresident_IsSecretary",
                table: "DefendCapstoneProjectCouncilMember");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ca886446-11d4-4ef5-9b9b-f91d01bf8b01"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2d14d263-e812-4be8-8345-659bc467e3bc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8ce58f2c-333b-4594-b2a9-df04802ae35b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("41cfb63c-8fc8-441b-88b5-e873f65cdb17"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 23, 16, 32, 49, 276, DateTimeKind.Local).AddTicks(6010),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 25, 3, 57, 44, 510, DateTimeKind.Local).AddTicks(1049));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4f7ba5a5-f018-4927-897c-b44a4cb52ebd"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("aded47bd-9211-4bfb-894f-bad525b29204"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("65d61e8d-37cc-44af-a1ab-b17f2915a30f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("298feeb7-da48-463f-bbed-1cb3bf540d00"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d5023726-1520-4aa9-ab8a-110583761046"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b62fb4e3-f19f-47d7-9f33-b51ab290617d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0a6e1b52-9892-4564-8ff9-910ea6328125"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("69ffd5bb-76f2-4385-83c1-62ed341615e9"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3d657f98-8360-45ff-b9ec-4505ecbec6dc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9aa17922-41e1-46ed-99a8-ac0b99c440fc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3b2ed125-f3ee-4c1d-b15b-f197974e1d01"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8be6515f-506a-427c-8b18-8f163456f67b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a3612480-c12b-47cf-94bb-26bf3430654d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3fc8d992-b4c1-4c28-b85f-dca5651b1261"));

            migrationBuilder.AlterColumn<string>(
                name: "Decision",
                table: "Group",
                type: "text",
                nullable: false,
                defaultValue: "Attempt1",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Undefined");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f59cc82b-12d5-4e45-b2f9-c76f9718b00f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("485a9aa9-6e5a-420c-b221-d4b1494be255"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("bc577149-2b4a-4842-acfc-b66932a6ec86"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("24ba4afe-b27c-4941-9fe6-ee4e3d1ed1c9"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("76ed6be6-cadb-46d2-a573-375d660c8241"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("111f48a9-796f-4a27-b831-6ba28034a01f"));
        }
    }
}
