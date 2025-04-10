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
                defaultValue: new Guid("4cfdf8ef-11a1-4035-8d75-b3af9668b38f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("85f2fa19-51db-4ab2-b1e1-7309e1a47dc5"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7bc98027-b3b1-4305-a92b-8b61147269df"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4b7165f3-a3d5-4ea7-b949-73d3585c0f6f"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 10, 18, 22, 58, 269, DateTimeKind.Local).AddTicks(4872),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 9, 11, 38, 0, 518, DateTimeKind.Local).AddTicks(3134));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("be8e7f43-c9d2-4612-b47f-a8a684e5d61a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0a94299c-c0f3-4533-b5db-192b75a36654"));

            migrationBuilder.AlterColumn<int>(
                name: "DifficultyLevel",
                table: "Topic",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8f77ddf1-b68b-4c4d-b5c9-bc717f9ef7c9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("1bae9d97-6cde-443a-8555-c8b128c86a16"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c7a11873-893c-4e71-86d0-c5a29391f975"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f8fff90c-3db3-4f64-baa2-190f2b22d57a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c14b10dd-0039-441f-bb12-2bb2148c436e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0de2d92c-80f3-4699-969f-bca294b61fa2"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f77e90b6-f1a8-4dc1-a786-0747279a7eb9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0bee00f1-7f71-41fc-97dc-ed1ea7eb3a77"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("74d53b16-2284-4d59-b3f6-7d3f4f1d7636"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9cdcd517-83f5-4bcf-84e1-a80ec3041d77"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b9274168-cb09-4114-bc4b-4f80bdcebf29"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9af13050-bc2e-4af9-871b-d6c2c32cd8d6"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("49bcc539-3eaa-4a8c-8d03-b074e73b07cb"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("1d05cce5-c7a5-4ade-af0e-46168f50ff63"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f36bde14-1118-41e1-8c11-ce0e7520c01a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4f251d8f-5337-49e8-885f-1bcb77f33fdd"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b49439b0-463b-4801-889c-70fe8127f557"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c5307b54-e3da-44d6-960f-6b62dde3b3ed"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("85f2fa19-51db-4ab2-b1e1-7309e1a47dc5"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4cfdf8ef-11a1-4035-8d75-b3af9668b38f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4b7165f3-a3d5-4ea7-b949-73d3585c0f6f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7bc98027-b3b1-4305-a92b-8b61147269df"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 9, 11, 38, 0, 518, DateTimeKind.Local).AddTicks(3134),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 4, 10, 18, 22, 58, 269, DateTimeKind.Local).AddTicks(4872));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0a94299c-c0f3-4533-b5db-192b75a36654"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("be8e7f43-c9d2-4612-b47f-a8a684e5d61a"));

            migrationBuilder.AlterColumn<string>(
                name: "DifficultyLevel",
                table: "Topic",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("1bae9d97-6cde-443a-8555-c8b128c86a16"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8f77ddf1-b68b-4c4d-b5c9-bc717f9ef7c9"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f8fff90c-3db3-4f64-baa2-190f2b22d57a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c7a11873-893c-4e71-86d0-c5a29391f975"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0de2d92c-80f3-4699-969f-bca294b61fa2"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c14b10dd-0039-441f-bb12-2bb2148c436e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0bee00f1-7f71-41fc-97dc-ed1ea7eb3a77"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f77e90b6-f1a8-4dc1-a786-0747279a7eb9"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9cdcd517-83f5-4bcf-84e1-a80ec3041d77"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("74d53b16-2284-4d59-b3f6-7d3f4f1d7636"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9af13050-bc2e-4af9-871b-d6c2c32cd8d6"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b9274168-cb09-4114-bc4b-4f80bdcebf29"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("1d05cce5-c7a5-4ade-af0e-46168f50ff63"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("49bcc539-3eaa-4a8c-8d03-b074e73b07cb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4f251d8f-5337-49e8-885f-1bcb77f33fdd"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f36bde14-1118-41e1-8c11-ce0e7520c01a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c5307b54-e3da-44d6-960f-6b62dde3b3ed"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b49439b0-463b-4801-889c-70fe8127f557"));
        }
    }
}
