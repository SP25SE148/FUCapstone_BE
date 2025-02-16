using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixdbchangepropertyatStudenttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupMember_StudentId",
                table: "GroupMember");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMember_StudentId",
                table: "GroupMember",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupMember_StudentId",
                table: "GroupMember");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMember_StudentId",
                table: "GroupMember",
                column: "StudentId",
                unique: true);
        }
    }
}
