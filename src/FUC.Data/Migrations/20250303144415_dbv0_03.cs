using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations;

/// <inheritdoc />
public partial class dbv0_03 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "TopicAppraisal",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("8b3e797b-8cce-4f1a-9a2a-8561087cf66f"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("cbd68f0e-da82-4eed-b8d6-24d702b2a81a"));

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedDate",
            table: "TopicAnalysis",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(2025, 3, 3, 14, 44, 14, 713, DateTimeKind.Utc).AddTicks(8012),
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldDefaultValue: new DateTime(2025, 2, 28, 7, 11, 11, 518, DateTimeKind.Utc).AddTicks(284));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "TopicAnalysis",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("ad311d7f-dc2e-4c2e-9622-7c3b41abb592"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("2bcc3b19-6f4d-41e6-97e6-873b3506a99f"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "Topic",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("73e1d637-7862-4e69-b48e-c2dc068ed86e"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("c736296c-a79d-4aa1-a2b1-60da7439c5aa"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "TemplateDocument",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("5ab414f5-5277-4056-873b-80300631fbe4"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("6f6b1591-6201-4169-860f-c8486b9ee426"));

        migrationBuilder.AddColumn<bool>(
            name: "IsFile",
            table: "TemplateDocument",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<Guid>(
            name: "ParentId",
            table: "TemplateDocument",
            type: "uuid",
            nullable: true);

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "SupervisorGroupAssignment",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("1d919744-1378-4816-ab4b-a741dd05ccbb"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("9fea3583-5a6e-4d1d-8df5-5f2b39f9981d"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "GroupMember",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("fd71a591-3c3c-491f-81d2-345fb9b7a273"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("350561c5-b2d4-483d-a45c-ce39883453ad"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "Group",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("e39ca355-abfe-41a0-8052-283d3f6c7727"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("cdcd84cd-8137-4847-afac-14f5f4cd4a1e"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "CoSupervisor",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("42002c2c-35e8-4ace-a420-ba78c6d5b5bf"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("6448fbf4-fd03-4a6c-a4e6-94db50092e09"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "BusinessArea",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("87db72ec-a498-4fe4-9eda-d5fc5cbfc749"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("752fe979-c670-456d-82de-c02b5bcf1833"));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsFile",
            table: "TemplateDocument");

        migrationBuilder.DropColumn(
            name: "ParentId",
            table: "TemplateDocument");

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "TopicAppraisal",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("cbd68f0e-da82-4eed-b8d6-24d702b2a81a"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("8b3e797b-8cce-4f1a-9a2a-8561087cf66f"));

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedDate",
            table: "TopicAnalysis",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(2025, 2, 28, 7, 11, 11, 518, DateTimeKind.Utc).AddTicks(284),
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldDefaultValue: new DateTime(2025, 3, 3, 14, 44, 14, 713, DateTimeKind.Utc).AddTicks(8012));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "TopicAnalysis",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("2bcc3b19-6f4d-41e6-97e6-873b3506a99f"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("ad311d7f-dc2e-4c2e-9622-7c3b41abb592"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "Topic",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("c736296c-a79d-4aa1-a2b1-60da7439c5aa"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("73e1d637-7862-4e69-b48e-c2dc068ed86e"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "TemplateDocument",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("6f6b1591-6201-4169-860f-c8486b9ee426"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("5ab414f5-5277-4056-873b-80300631fbe4"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "SupervisorGroupAssignment",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("9fea3583-5a6e-4d1d-8df5-5f2b39f9981d"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("1d919744-1378-4816-ab4b-a741dd05ccbb"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "GroupMember",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("350561c5-b2d4-483d-a45c-ce39883453ad"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("fd71a591-3c3c-491f-81d2-345fb9b7a273"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "Group",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("cdcd84cd-8137-4847-afac-14f5f4cd4a1e"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("e39ca355-abfe-41a0-8052-283d3f6c7727"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "CoSupervisor",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("6448fbf4-fd03-4a6c-a4e6-94db50092e09"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("42002c2c-35e8-4ace-a420-ba78c6d5b5bf"));

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "BusinessArea",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("752fe979-c670-456d-82de-c02b5bcf1833"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldDefaultValue: new Guid("87db72ec-a498-4fe4-9eda-d5fc5cbfc749"));
    }
}
