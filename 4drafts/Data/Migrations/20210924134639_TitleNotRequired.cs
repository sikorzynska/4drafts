using Microsoft.EntityFrameworkCore.Migrations;

namespace _4drafts.Data.Migrations
{
    public partial class TitleNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Threads",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AddColumn<int>(
                name: "ThreadTypeId",
                table: "Drafts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Drafts_ThreadTypeId",
                table: "Drafts",
                column: "ThreadTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drafts_ThreadTypes_ThreadTypeId",
                table: "Drafts",
                column: "ThreadTypeId",
                principalTable: "ThreadTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drafts_ThreadTypes_ThreadTypeId",
                table: "Drafts");

            migrationBuilder.DropIndex(
                name: "IX_Drafts_ThreadTypeId",
                table: "Drafts");

            migrationBuilder.DropColumn(
                name: "ThreadTypeId",
                table: "Drafts");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Threads",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);
        }
    }
}
