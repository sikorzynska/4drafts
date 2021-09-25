using Microsoft.EntityFrameworkCore.Migrations;

namespace _4drafts.Data.Migrations
{
    public partial class drafttype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drafts_ThreadTypes_ThreadTypeId",
                table: "Drafts");

            migrationBuilder.AddForeignKey(
                name: "FK_Drafts_ThreadTypes_ThreadTypeId",
                table: "Drafts",
                column: "ThreadTypeId",
                principalTable: "ThreadTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drafts_ThreadTypes_ThreadTypeId",
                table: "Drafts");

            migrationBuilder.AddForeignKey(
                name: "FK_Drafts_ThreadTypes_ThreadTypeId",
                table: "Drafts",
                column: "ThreadTypeId",
                principalTable: "ThreadTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
