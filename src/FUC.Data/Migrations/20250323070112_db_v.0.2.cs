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
            migrationBuilder.DropColumn(
                name: "TopicCode",
                table: "Group");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d16960ed-e2e2-499a-960a-3a249c4dce43"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ff35c242-5457-44f8-b228-95bce47a42dc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d11208ad-82da-42e0-b2fe-2016b6347bab"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0309eec4-0805-4670-b76c-4fd37ee4e5db"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 23, 14, 1, 11, 979, DateTimeKind.Local).AddTicks(2747),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 21, 20, 15, 59, 146, DateTimeKind.Local).AddTicks(3725));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c3db238a-94fa-4baa-8774-7cf72ecaa5cb"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("59d9ef3b-9ab8-41ec-a840-c168c48af30c"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3fddb133-050e-4e16-8d1b-fd742a8e3a32"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c311b1fd-d1b0-4db1-9fff-2fe4a6f0f43a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f23bebc7-482a-4f68-ba60-d0251c1ab87a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a1504981-373c-43dc-87f7-902b8741b4b7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("95056ee9-487d-4399-b6dc-a40cd5d9990e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4ec6ae6a-8060-4282-af9e-021a9cffa6db"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8c468168-86ae-45a9-9fea-e8f9b8e1a84e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("bf8499b1-12e7-4376-b279-6cb35939c9c0"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("68a59f55-97e3-436a-a735-6a414fe82432"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b5d67d1b-6995-4da5-b20d-e86692ff199e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c901b4b0-aa54-4d8e-a72c-dd9516a14530"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3d8a915e-8360-438a-9156-28c284d01e01"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("73faee60-57dd-47da-91a0-d258911e15c3"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0ab94658-c249-4dc8-bd96-46f0d13eaea9"));

            migrationBuilder.AddColumn<Guid>(
                name: "TopicId",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("eb3e66f7-0039-44ad-9d79-789a4d89ebee"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0440d914-71e3-408a-a7cb-cbb0e0e31191"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("164f52e3-4ff9-4020-8f16-a1dc324055a5"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("836ecfa8-172c-4a72-a0ba-6a11f4af21ba"));

            migrationBuilder.CreateIndex(
                name: "IX_Group_TopicId",
                table: "Group",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Topic_TopicId",
                table: "Group",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Topic_TopicId",
                table: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Group_TopicId",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Group");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ff35c242-5457-44f8-b228-95bce47a42dc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d16960ed-e2e2-499a-960a-3a249c4dce43"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0309eec4-0805-4670-b76c-4fd37ee4e5db"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d11208ad-82da-42e0-b2fe-2016b6347bab"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 21, 20, 15, 59, 146, DateTimeKind.Local).AddTicks(3725),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 23, 14, 1, 11, 979, DateTimeKind.Local).AddTicks(2747));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("59d9ef3b-9ab8-41ec-a840-c168c48af30c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c3db238a-94fa-4baa-8774-7cf72ecaa5cb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c311b1fd-d1b0-4db1-9fff-2fe4a6f0f43a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3fddb133-050e-4e16-8d1b-fd742a8e3a32"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a1504981-373c-43dc-87f7-902b8741b4b7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f23bebc7-482a-4f68-ba60-d0251c1ab87a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4ec6ae6a-8060-4282-af9e-021a9cffa6db"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("95056ee9-487d-4399-b6dc-a40cd5d9990e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("bf8499b1-12e7-4376-b279-6cb35939c9c0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8c468168-86ae-45a9-9fea-e8f9b8e1a84e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b5d67d1b-6995-4da5-b20d-e86692ff199e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("68a59f55-97e3-436a-a735-6a414fe82432"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3d8a915e-8360-438a-9156-28c284d01e01"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c901b4b0-aa54-4d8e-a72c-dd9516a14530"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0ab94658-c249-4dc8-bd96-46f0d13eaea9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("73faee60-57dd-47da-91a0-d258911e15c3"));

            migrationBuilder.AddColumn<string>(
                name: "TopicCode",
                table: "Group",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0440d914-71e3-408a-a7cb-cbb0e0e31191"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("eb3e66f7-0039-44ad-9d79-789a4d89ebee"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("836ecfa8-172c-4a72-a0ba-6a11f4af21ba"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("164f52e3-4ff9-4020-8f16-a1dc324055a5"));
        }
    }
}
