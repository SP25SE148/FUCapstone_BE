using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class db_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Topic_TopicId",
                table: "Group");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d20e461e-c2c5-48ec-ab6c-95d978c9d303"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("51d6815c-1166-4198-afbc-5c1c00012794"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5d811355-50ef-404a-b25f-1e3983a71edc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3cd12959-eaf1-44c6-9039-b8665c01ca9e"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 8, 9, 31, 19, 825, DateTimeKind.Local).AddTicks(8576),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 5, 18, 51, 18, 475, DateTimeKind.Local).AddTicks(5741));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3da051ce-b5e3-401b-9ba3-65df03c5bee1"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("67903b0b-1c6b-4c06-ace5-bebb9c44283e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("bda1b694-18dd-4cd8-8862-ba5633dd4463"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("6442da0c-6d7c-4f03-9d60-72fcdaac09e8"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d3e2b9cd-1223-40c7-86d4-3da91057725d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("67f94971-0c2d-44d7-8c92-04319dd854fd"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e2a2f043-4a4b-45c9-b1a4-8eda3a6bfec7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9a91abeb-0229-4033-be2e-7e5c4258d927"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("dd68758e-9a40-4ed9-81ab-38c5096b1963"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("17e143a0-cf42-4ebf-a126-568b1966a456"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("05b1e57a-7176-47c0-aac3-29d85c3d74d9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f75f83a3-8d07-4695-8263-106892c39a83"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8cebdcb2-b918-4faa-ae8a-bde3d418a37f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("73bfba70-8f8e-4cde-8a2e-c90984c2a05f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ff7990ce-070a-403a-9947-1b2507ebd3f7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("fe769a6b-b4da-48e4-99fd-38ee376a1c13"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f01946a9-7672-4646-80bc-5d6a1801781f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8f2c5363-eb5a-4fc8-9100-688fc05d6466"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7823fe74-75de-4835-be18-3736a14b3091"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c29d489f-94b6-4895-8457-79dc1de02911"));

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Topic_TopicId",
                table: "Group",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Topic_TopicId",
                table: "Group");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("51d6815c-1166-4198-afbc-5c1c00012794"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d20e461e-c2c5-48ec-ab6c-95d978c9d303"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3cd12959-eaf1-44c6-9039-b8665c01ca9e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("5d811355-50ef-404a-b25f-1e3983a71edc"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 5, 18, 51, 18, 475, DateTimeKind.Local).AddTicks(5741),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 8, 9, 31, 19, 825, DateTimeKind.Local).AddTicks(8576));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("67903b0b-1c6b-4c06-ace5-bebb9c44283e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3da051ce-b5e3-401b-9ba3-65df03c5bee1"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("6442da0c-6d7c-4f03-9d60-72fcdaac09e8"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("bda1b694-18dd-4cd8-8862-ba5633dd4463"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("67f94971-0c2d-44d7-8c92-04319dd854fd"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d3e2b9cd-1223-40c7-86d4-3da91057725d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9a91abeb-0229-4033-be2e-7e5c4258d927"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e2a2f043-4a4b-45c9-b1a4-8eda3a6bfec7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("17e143a0-cf42-4ebf-a126-568b1966a456"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("dd68758e-9a40-4ed9-81ab-38c5096b1963"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f75f83a3-8d07-4695-8263-106892c39a83"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("05b1e57a-7176-47c0-aac3-29d85c3d74d9"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("73bfba70-8f8e-4cde-8a2e-c90984c2a05f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8cebdcb2-b918-4faa-ae8a-bde3d418a37f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("fe769a6b-b4da-48e4-99fd-38ee376a1c13"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ff7990ce-070a-403a-9947-1b2507ebd3f7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8f2c5363-eb5a-4fc8-9100-688fc05d6466"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f01946a9-7672-4646-80bc-5d6a1801781f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c29d489f-94b6-4895-8457-79dc1de02911"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7823fe74-75de-4835-be18-3736a14b3091"));

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Topic_TopicId",
                table: "Group",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
