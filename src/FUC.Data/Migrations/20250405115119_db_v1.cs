using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class db_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("51d6815c-1166-4198-afbc-5c1c00012794"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("1fbaec46-d915-425a-b462-ca367e1c53ad"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3cd12959-eaf1-44c6-9039-b8665c01ca9e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b8e68940-5670-4ea2-80ab-087c7592dc94"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 5, 18, 51, 18, 475, DateTimeKind.Local).AddTicks(5741),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 3, 17, 1, 44, 301, DateTimeKind.Local).AddTicks(612));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("67903b0b-1c6b-4c06-ace5-bebb9c44283e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("27b511a7-7bd5-461a-af66-476995d32975"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("6442da0c-6d7c-4f03-9d60-72fcdaac09e8"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("128983e3-a645-4369-9ef3-61d8b55648ba"));

            migrationBuilder.AddColumn<string>(
                name: "GroupCode",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("67f94971-0c2d-44d7-8c92-04319dd854fd"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7f8222fc-2a18-428e-a239-666b0ca34fda"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9a91abeb-0229-4033-be2e-7e5c4258d927"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("97f183ce-3e18-4d84-a732-c11280876383"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("17e143a0-cf42-4ebf-a126-568b1966a456"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4a0a7f46-01c9-4fed-ad0c-adc3e231e34d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f75f83a3-8d07-4695-8263-106892c39a83"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("acc7f64d-9f42-401d-b5b3-7281eeabed73"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("73bfba70-8f8e-4cde-8a2e-c90984c2a05f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e7d93f02-fc61-42dd-a074-7c624b38190a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("fe769a6b-b4da-48e4-99fd-38ee376a1c13"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8b75eab4-2190-4668-8159-b08184e90918"));

            migrationBuilder.AddColumn<bool>(
                name: "IsUploadGroupDocument",
                table: "Group",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8f2c5363-eb5a-4fc8-9100-688fc05d6466"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a9246452-7f75-47f1-8dcc-83b398a4cc2d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c29d489f-94b6-4895-8457-79dc1de02911"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("66ec4c48-0334-43e6-ab2d-f009d2e587d1"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "IsUploadGroupDocument",
                table: "Group");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("1fbaec46-d915-425a-b462-ca367e1c53ad"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("51d6815c-1166-4198-afbc-5c1c00012794"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b8e68940-5670-4ea2-80ab-087c7592dc94"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3cd12959-eaf1-44c6-9039-b8665c01ca9e"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 3, 17, 1, 44, 301, DateTimeKind.Local).AddTicks(612),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 5, 18, 51, 18, 475, DateTimeKind.Local).AddTicks(5741));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("27b511a7-7bd5-461a-af66-476995d32975"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("67903b0b-1c6b-4c06-ace5-bebb9c44283e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("128983e3-a645-4369-9ef3-61d8b55648ba"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("6442da0c-6d7c-4f03-9d60-72fcdaac09e8"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7f8222fc-2a18-428e-a239-666b0ca34fda"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("67f94971-0c2d-44d7-8c92-04319dd854fd"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("97f183ce-3e18-4d84-a732-c11280876383"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9a91abeb-0229-4033-be2e-7e5c4258d927"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4a0a7f46-01c9-4fed-ad0c-adc3e231e34d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("17e143a0-cf42-4ebf-a126-568b1966a456"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("acc7f64d-9f42-401d-b5b3-7281eeabed73"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f75f83a3-8d07-4695-8263-106892c39a83"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e7d93f02-fc61-42dd-a074-7c624b38190a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("73bfba70-8f8e-4cde-8a2e-c90984c2a05f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8b75eab4-2190-4668-8159-b08184e90918"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("fe769a6b-b4da-48e4-99fd-38ee376a1c13"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a9246452-7f75-47f1-8dcc-83b398a4cc2d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8f2c5363-eb5a-4fc8-9100-688fc05d6466"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("66ec4c48-0334-43e6-ab2d-f009d2e587d1"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c29d489f-94b6-4895-8457-79dc1de02911"));
        }
    }
}
