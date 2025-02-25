using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations;

/// <inheritdoc />
public partial class db_v004 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DeletedAt",
            table: "TemplateDocument");

        migrationBuilder.RenameColumn(
            name: "IsDeleted",
            table: "TemplateDocument",
            newName: "IsActive");

        migrationBuilder.AddColumn<bool>(
            name: "IsAvailable",
            table: "Supervisor",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "TopicAnalysis",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AnalysisResult = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 2, 25, 11, 15, 10, 452, DateTimeKind.Utc).AddTicks(6087)),
                TopicId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TopicAnalysis", x => x.Id);
                table.ForeignKey(
                    name: "FK_TopicAnalysis_Topic_TopicId",
                    column: x => x.TopicId,
                    principalTable: "Topic",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_TopicAnalysis_TopicId",
            table: "TopicAnalysis",
            column: "TopicId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TopicAnalysis");

        migrationBuilder.DropColumn(
            name: "IsAvailable",
            table: "Supervisor");

        migrationBuilder.RenameColumn(
            name: "IsActive",
            table: "TemplateDocument",
            newName: "IsDeleted");

        migrationBuilder.AddColumn<DateTime>(
            name: "DeletedAt",
            table: "TemplateDocument",
            type: "timestamp with time zone",
            nullable: true);
    }
}
