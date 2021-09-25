using Microsoft.EntityFrameworkCore.Migrations;

namespace _4drafts.Data.Migrations
{
    public partial class DraftPromptId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PromptId",
                table: "Drafts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromptId",
                table: "Drafts");
        }
    }
}
