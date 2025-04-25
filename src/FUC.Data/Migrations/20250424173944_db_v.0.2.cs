using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class db_v02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeConfiguration_SemesterId",
                table: "TimeConfiguration");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f8bd3d7a-c940-4526-b65d-2953b6b63146"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("26970534-6c79-4139-a21c-0fd6929757cb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("24edfbcf-83e8-4479-84c1-4b9cf0a4220c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("fb918f3e-241f-497a-9196-e9f4ed0e02b1"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 25, 0, 39, 43, 912, DateTimeKind.Local).AddTicks(1622),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 25, 0, 6, 52, 577, DateTimeKind.Local).AddTicks(8858));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("27870903-3663-4200-bd95-4fb3b0cb286c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b470377e-6246-4a9d-87e7-bd0f90a8aa62"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c77ab4b2-d03d-4ef6-9312-2ed7c7721dcc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e4cf9fb5-4858-4a4d-90d8-06a9cbc12a87"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3400b79b-b4f5-4d52-8f9d-2e46e0b06c23"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("90bcf4c7-d314-4cc9-909d-0f187a693a17"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4f3734ed-48ea-47de-9366-24a3a15a9baf"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("354a1f71-f64f-4dd3-ae05-27a5604170ab"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("723941dc-ab55-4cda-b4b3-d82417ad99e7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ed41d36f-52c6-4c8b-b84a-5b48ef74ca88"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("10295006-b8f9-4be0-b39d-5b74f9f9c778"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("665066f0-a7fd-457a-931c-450c3ac86f35"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("edc8d66d-2b07-4a81-a12c-408ea005ac7a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e6ac3390-aed1-4962-bc41-256a2f5ac2d3"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ed6f2d37-af38-4cf6-93f8-b38237a83aee"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9d6af0e8-e957-4621-bc8d-81fe81fdd73f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("024b306c-56e1-47f8-a2f2-32f947a21919"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ada1a2d2-7674-408c-99b9-0ada0ff9a75d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("25728c7d-d1d0-41ad-b2b3-6238c52e1a13"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7c4b1de0-1aa0-492d-ae9f-c219a896400e"));

            migrationBuilder.CreateIndex(
                name: "IX_TimeConfiguration_SemesterId",
                table: "TimeConfiguration",
                column: "SemesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeConfiguration_SemesterId",
                table: "TimeConfiguration");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("26970534-6c79-4139-a21c-0fd6929757cb"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f8bd3d7a-c940-4526-b65d-2953b6b63146"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("fb918f3e-241f-497a-9196-e9f4ed0e02b1"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("24edfbcf-83e8-4479-84c1-4b9cf0a4220c"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 25, 0, 6, 52, 577, DateTimeKind.Local).AddTicks(8858),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 25, 0, 39, 43, 912, DateTimeKind.Local).AddTicks(1622));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b470377e-6246-4a9d-87e7-bd0f90a8aa62"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("27870903-3663-4200-bd95-4fb3b0cb286c"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e4cf9fb5-4858-4a4d-90d8-06a9cbc12a87"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c77ab4b2-d03d-4ef6-9312-2ed7c7721dcc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("90bcf4c7-d314-4cc9-909d-0f187a693a17"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3400b79b-b4f5-4d52-8f9d-2e46e0b06c23"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("354a1f71-f64f-4dd3-ae05-27a5604170ab"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4f3734ed-48ea-47de-9366-24a3a15a9baf"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ed41d36f-52c6-4c8b-b84a-5b48ef74ca88"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("723941dc-ab55-4cda-b4b3-d82417ad99e7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("665066f0-a7fd-457a-931c-450c3ac86f35"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("10295006-b8f9-4be0-b39d-5b74f9f9c778"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e6ac3390-aed1-4962-bc41-256a2f5ac2d3"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("edc8d66d-2b07-4a81-a12c-408ea005ac7a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9d6af0e8-e957-4621-bc8d-81fe81fdd73f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ed6f2d37-af38-4cf6-93f8-b38237a83aee"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ada1a2d2-7674-408c-99b9-0ada0ff9a75d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("024b306c-56e1-47f8-a2f2-32f947a21919"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7c4b1de0-1aa0-492d-ae9f-c219a896400e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("25728c7d-d1d0-41ad-b2b3-6238c52e1a13"));

            migrationBuilder.CreateIndex(
                name: "IX_TimeConfiguration_SemesterId",
                table: "TimeConfiguration",
                column: "SemesterId",
                unique: true);
        }
    }
}
