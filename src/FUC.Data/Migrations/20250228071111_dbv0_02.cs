using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class dbv0_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AppraisalDate",
                table: "TopicAppraisal",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "AppraisalContent",
                table: "TopicAppraisal",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AppraisalComment",
                table: "TopicAppraisal",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("cbd68f0e-da82-4eed-b8d6-24d702b2a81a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2f4ee256-67a1-4254-8a89-29cf63b7252a"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 2, 28, 7, 11, 11, 518, DateTimeKind.Utc).AddTicks(284),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 2, 28, 6, 19, 50, 650, DateTimeKind.Utc).AddTicks(6159));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2bcc3b19-6f4d-41e6-97e6-873b3506a99f"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("bde06dc9-2d25-48ca-9a1b-c67578753108"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c736296c-a79d-4aa1-a2b1-60da7439c5aa"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("89fabba3-b356-4b73-871d-8323327079d4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("6f6b1591-6201-4169-860f-c8486b9ee426"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("b7cb605b-6fd5-4361-99f7-58c8c3a0aae9"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SupervisorGroupAssignment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("9fea3583-5a6e-4d1d-8df5-5f2b39f9981d"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c74baf6a-d87b-4830-93e6-779e04c136ca"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("350561c5-b2d4-483d-a45c-ce39883453ad"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("45ac631c-43b3-4ccd-bcdb-a7aea60e4236"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("cdcd84cd-8137-4847-afac-14f5f4cd4a1e"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("02791dec-015b-4985-aeb8-d02bcc88eaa2"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("6448fbf4-fd03-4a6c-a4e6-94db50092e09"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("46e0c312-a633-4034-bdbe-583b4c772605"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("752fe979-c670-456d-82de-c02b5bcf1833"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("17673ba7-4c03-49d4-b870-927e40776102"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AppraisalDate",
                table: "TopicAppraisal",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AppraisalContent",
                table: "TopicAppraisal",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AppraisalComment",
                table: "TopicAppraisal",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAppraisal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("2f4ee256-67a1-4254-8a89-29cf63b7252a"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("cbd68f0e-da82-4eed-b8d6-24d702b2a81a"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TopicAnalysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 2, 28, 6, 19, 50, 650, DateTimeKind.Utc).AddTicks(6159),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 2, 28, 7, 11, 11, 518, DateTimeKind.Utc).AddTicks(284));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TopicAnalysis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("bde06dc9-2d25-48ca-9a1b-c67578753108"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("2bcc3b19-6f4d-41e6-97e6-873b3506a99f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Topic",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("89fabba3-b356-4b73-871d-8323327079d4"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("c736296c-a79d-4aa1-a2b1-60da7439c5aa"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemplateDocument",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("b7cb605b-6fd5-4361-99f7-58c8c3a0aae9"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("6f6b1591-6201-4169-860f-c8486b9ee426"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SupervisorGroupAssignment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("c74baf6a-d87b-4830-93e6-779e04c136ca"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("9fea3583-5a6e-4d1d-8df5-5f2b39f9981d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMember",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("45ac631c-43b3-4ccd-bcdb-a7aea60e4236"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("350561c5-b2d4-483d-a45c-ce39883453ad"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Group",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("02791dec-015b-4985-aeb8-d02bcc88eaa2"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("cdcd84cd-8137-4847-afac-14f5f4cd4a1e"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CoSupervisor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("46e0c312-a633-4034-bdbe-583b4c772605"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("6448fbf4-fd03-4a6c-a4e6-94db50092e09"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BusinessArea",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("17673ba7-4c03-49d4-b870-927e40776102"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("752fe979-c670-456d-82de-c02b5bcf1833"));
        }
    }
}
