using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Db_v02 : Migration
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
                defaultValue: new Guid("878553d0-3d96-4461-8fd5-33f90645da02"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ff35c242-5457-44f8-b228-95bce47a42dc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2264541a-6308-4776-91e3-df37d4a568ae"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0309eec4-0805-4670-b76c-4fd37ee4e5db"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 23, 14, 16, 1, 388, DateTimeKind.Local).AddTicks(1307),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 21, 20, 15, 59, 146, DateTimeKind.Local).AddTicks(3725));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f99a2e1f-0b98-4d93-885d-07cf713b40cc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("59d9ef3b-9ab8-41ec-a840-c168c48af30c"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("f35f2fee-c390-44b6-b872-bb98f0be6e81"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c311b1fd-d1b0-4db1-9fff-2fe4a6f0f43a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7738332d-d0bf-4c7e-9b5e-39333a7be90e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a1504981-373c-43dc-87f7-902b8741b4b7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9a6bf852-b996-4f75-b748-58b3184c3fa2"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("4ec6ae6a-8060-4282-af9e-021a9cffa6db"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("7fba7c89-19fc-4ea2-be58-fc3c155da55c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("bf8499b1-12e7-4376-b279-6cb35939c9c0"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("12c1f94c-9c81-4d8c-b53d-ad19f8a079fd"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b5d67d1b-6995-4da5-b20d-e86692ff199e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("39a0fb9c-b62b-4bc8-8d97-89598cefc4a8"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3d8a915e-8360-438a-9156-28c284d01e01"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c08d8cc8-ed29-4328-9f50-c326c7bb9d88"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0ab94658-c249-4dc8-bd96-46f0d13eaea9"));

            migrationBuilder.AddColumn<Guid>(
                name: "TopicId",
                table: "Group",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5abcb688-426a-4c10-a3e7-28b5b268088e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("0440d914-71e3-408a-a7cb-cbb0e0e31191"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b896b47d-4b12-44d5-91c9-7c2e7857a3dc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("836ecfa8-172c-4a72-a0ba-6a11f4af21ba"));

            migrationBuilder.CreateIndex(
                name: "IX_Group_TopicId",
                table: "Group",
                column: "TopicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Topic_TopicId",
                table: "Group",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
                oldDefaultValue: new Guid("878553d0-3d96-4461-8fd5-33f90645da02"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0309eec4-0805-4670-b76c-4fd37ee4e5db"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2264541a-6308-4776-91e3-df37d4a568ae"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 21, 20, 15, 59, 146, DateTimeKind.Local).AddTicks(3725),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 23, 14, 16, 1, 388, DateTimeKind.Local).AddTicks(1307));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("59d9ef3b-9ab8-41ec-a840-c168c48af30c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f99a2e1f-0b98-4d93-885d-07cf713b40cc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c311b1fd-d1b0-4db1-9fff-2fe4a6f0f43a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("f35f2fee-c390-44b6-b872-bb98f0be6e81"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a1504981-373c-43dc-87f7-902b8741b4b7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7738332d-d0bf-4c7e-9b5e-39333a7be90e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("4ec6ae6a-8060-4282-af9e-021a9cffa6db"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9a6bf852-b996-4f75-b748-58b3184c3fa2"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("bf8499b1-12e7-4376-b279-6cb35939c9c0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("7fba7c89-19fc-4ea2-be58-fc3c155da55c"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b5d67d1b-6995-4da5-b20d-e86692ff199e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("12c1f94c-9c81-4d8c-b53d-ad19f8a079fd"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3d8a915e-8360-438a-9156-28c284d01e01"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("39a0fb9c-b62b-4bc8-8d97-89598cefc4a8"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0ab94658-c249-4dc8-bd96-46f0d13eaea9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c08d8cc8-ed29-4328-9f50-c326c7bb9d88"));

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
                oldDefaultValue: new Guid("5abcb688-426a-4c10-a3e7-28b5b268088e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("836ecfa8-172c-4a72-a0ba-6a11f4af21ba"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b896b47d-4b12-44d5-91c9-7c2e7857a3dc"));
        }
    }
}
