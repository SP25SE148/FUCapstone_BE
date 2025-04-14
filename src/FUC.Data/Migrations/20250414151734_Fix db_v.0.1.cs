using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fixdb_v01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slot",
                table: "ReviewCalendar");

            migrationBuilder.DropColumn(
                name: "Slot",
                table: "DefendCapstoneProjectInformationCalendar");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f3ca3577-794b-44dc-b2b5-3767ed7d4a88"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c0061bc3-5939-4cb7-9c0b-6a8f6b333bb1"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9f2f3c72-7682-4ac5-b803-8a333eac608d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("6e861ba6-0d86-4be1-a675-6e598e17154f"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 14, 22, 17, 33, 714, DateTimeKind.Local).AddTicks(3466),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 12, 23, 7, 7, 36, DateTimeKind.Local).AddTicks(8368));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2cf64196-10b4-4013-b82f-53ec0f40e8f6"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("42d54ac9-3f8a-4cd5-923a-0be29f903813"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3bac3a6b-01e9-4f4f-a044-fab8aaf6f7e6"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ee087e5a-14dd-402a-a51a-3627c05f5c56"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("14a0147e-868a-45d4-8b93-2e34950b96b2"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c3443aeb-a5cb-46ce-af03-5b1f08b892dc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("025d89f5-f35e-4c92-8ed1-d413436f3afc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a4c73af3-981c-488e-82d8-1536918012f1"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ba0e5ec7-6388-4a72-84d1-bf3701cbe683"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3fd62965-a0f2-4b29-bfb0-bc53a086dcf2"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e46befa9-b51e-4e1a-b351-4efd682690bd"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2ea3f362-2db7-42c5-9a14-97e18f7126e3"));

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "ReviewCalendar",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("74604ea6-468b-496a-8905-6860968b7bdc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9fd01583-3db2-4728-bea0-25d7dc7c711c"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("76a5a344-fc0f-4cd8-88d1-9384d5401c86"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("96f869be-2763-424a-9746-c32ebdf5db7c"));

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "DefendCapstoneProjectInformationCalendar",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2a90c565-7c05-489e-864b-8f6e43d4e40a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b245caf1-9c2e-4fd0-8fae-2f0550c2da90"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8ba21931-b4a2-4f09-a22d-b64be857fdc7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8ccbf42e-e5c4-40a2-81dd-06c29afc79aa"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "ReviewCalendar");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "DefendCapstoneProjectInformationCalendar");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c0061bc3-5939-4cb7-9c0b-6a8f6b333bb1"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f3ca3577-794b-44dc-b2b5-3767ed7d4a88"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("6e861ba6-0d86-4be1-a675-6e598e17154f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9f2f3c72-7682-4ac5-b803-8a333eac608d"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 12, 23, 7, 7, 36, DateTimeKind.Local).AddTicks(8368),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 14, 22, 17, 33, 714, DateTimeKind.Local).AddTicks(3466));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("42d54ac9-3f8a-4cd5-923a-0be29f903813"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2cf64196-10b4-4013-b82f-53ec0f40e8f6"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ee087e5a-14dd-402a-a51a-3627c05f5c56"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3bac3a6b-01e9-4f4f-a044-fab8aaf6f7e6"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c3443aeb-a5cb-46ce-af03-5b1f08b892dc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("14a0147e-868a-45d4-8b93-2e34950b96b2"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a4c73af3-981c-488e-82d8-1536918012f1"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("025d89f5-f35e-4c92-8ed1-d413436f3afc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3fd62965-a0f2-4b29-bfb0-bc53a086dcf2"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ba0e5ec7-6388-4a72-84d1-bf3701cbe683"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2ea3f362-2db7-42c5-9a14-97e18f7126e3"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e46befa9-b51e-4e1a-b351-4efd682690bd"));

            migrationBuilder.AddColumn<int>(
                name: "Slot",
                table: "ReviewCalendar",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9fd01583-3db2-4728-bea0-25d7dc7c711c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("74604ea6-468b-496a-8905-6860968b7bdc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("96f869be-2763-424a-9746-c32ebdf5db7c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("76a5a344-fc0f-4cd8-88d1-9384d5401c86"));

            migrationBuilder.AddColumn<int>(
                name: "Slot",
                table: "DefendCapstoneProjectInformationCalendar",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b245caf1-9c2e-4fd0-8fae-2f0550c2da90"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2a90c565-7c05-489e-864b-8f6e43d4e40a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8ccbf42e-e5c4-40a2-81dd-06c29afc79aa"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8ba21931-b4a2-4f09-a22d-b64be857fdc7"));
        }
    }
}
