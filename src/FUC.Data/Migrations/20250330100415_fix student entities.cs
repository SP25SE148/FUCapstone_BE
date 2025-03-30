using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixstudententities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEligible",
                table: "Student");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c54fd789-3f60-4c29-a41e-d91e08255b20"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("5918ba30-bc1f-4241-859d-1a02b00f5fc6"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("22d24b58-b77b-4799-8f39-a2bf39ec33f0"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("6b4666a8-6128-4bad-9215-11c94a4418ce"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 30, 17, 4, 15, 117, DateTimeKind.Local).AddTicks(7177),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 28, 14, 14, 43, 5, DateTimeKind.Local).AddTicks(264));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("02ef524a-879a-4a2a-bedf-39a7eba8f05d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a1c28b67-61ba-4b75-a25d-de0db69fba4a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c31b579e-75a0-4586-9ee7-59a6915426a3"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c915e670-101e-42ea-b372-6635777dd8ff"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3a55c7d6-db18-460a-a857-94b085ee09c4"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("a5b40fc1-2bb6-441f-b872-5da44d7a4a45"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3e85a74a-cc46-416e-89b7-566d4bb9b502"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("ac8bb195-9bf8-401b-8c17-1a6ff684489b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2ebc63c8-d92c-45f7-acc8-6b8d257bcd6a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9ef64adf-65dd-4043-a6ee-b30d67bce2ef"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("8ec6d2fc-816b-4c1d-b01d-373d44efdd3d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("46aeae45-0ea5-4661-8f42-e87f15637013"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("324e6295-d8f1-4b13-a1b6-112df04281e7"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("63b9f8fc-89c9-4d67-af92-1fc6d1bd893f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d5d3f7ad-45ba-4deb-a267-263014b3fcdc"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("327e7b39-3d0e-4291-8eb6-c3b6fe962796"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("d8628df1-3757-4665-b70f-ac5e3c0a9e58"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c947df81-e67c-4539-a5bc-134b813ca661"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c980796b-2280-42b6-a4b4-bcc1b5f3aea9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("5807892d-4dcb-45cc-8cd9-972985c5ad15"));

            migrationBuilder.CreateTable(
                name: "TimeConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamUpDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    TeamUpExpirationDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    RegistTopicDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    RegistTopicExpiredDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    IsActived = table.Column<bool>(type: "boolean", nullable: false),
                    CampusId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeConfiguration_Campus_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeConfiguration_CampusId",
                table: "TimeConfiguration",
                column: "CampusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeConfiguration");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5918ba30-bc1f-4241-859d-1a02b00f5fc6"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c54fd789-3f60-4c29-a41e-d91e08255b20"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("6b4666a8-6128-4bad-9215-11c94a4418ce"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("22d24b58-b77b-4799-8f39-a2bf39ec33f0"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 28, 14, 14, 43, 5, DateTimeKind.Local).AddTicks(264),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2025, 3, 30, 17, 4, 15, 117, DateTimeKind.Local).AddTicks(7177));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a1c28b67-61ba-4b75-a25d-de0db69fba4a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("02ef524a-879a-4a2a-bedf-39a7eba8f05d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c915e670-101e-42ea-b372-6635777dd8ff"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c31b579e-75a0-4586-9ee7-59a6915426a3"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("a5b40fc1-2bb6-441f-b872-5da44d7a4a45"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3a55c7d6-db18-460a-a857-94b085ee09c4"));

            migrationBuilder.AddColumn<bool>(
                name: "IsEligible",
                table: "Student",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reviewer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("ac8bb195-9bf8-401b-8c17-1a6ff684489b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3e85a74a-cc46-416e-89b7-566d4bb9b502"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCriteria",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9ef64adf-65dd-4043-a6ee-b30d67bce2ef"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2ebc63c8-d92c-45f7-acc8-6b8d257bcd6a"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReviewCalendar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("46aeae45-0ea5-4661-8f42-e87f15637013"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("8ec6d2fc-816b-4c1d-b01d-373d44efdd3d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("63b9f8fc-89c9-4d67-af92-1fc6d1bd893f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("324e6295-d8f1-4b13-a1b6-112df04281e7"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("327e7b39-3d0e-4291-8eb6-c3b6fe962796"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d5d3f7ad-45ba-4deb-a267-263014b3fcdc"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c947df81-e67c-4539-a5bc-134b813ca661"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("d8628df1-3757-4665-b70f-ac5e3c0a9e58"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5807892d-4dcb-45cc-8cd9-972985c5ad15"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c980796b-2280-42b6-a4b4-bcc1b5f3aea9"));
        }
    }
}
