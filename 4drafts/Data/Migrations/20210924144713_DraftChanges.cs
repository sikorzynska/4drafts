using Microsoft.EntityFrameworkCore.Migrations;

namespace _4drafts.Data.Migrations
{
    public partial class DraftChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Drafts_DraftId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_DraftId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "DraftId",
                table: "Genres");

            migrationBuilder.AddColumn<int>(
                name: "FirstGenre",
                table: "Drafts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondGenre",
                table: "Drafts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThirdGenre",
                table: "Drafts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstGenre",
                table: "Drafts");

            migrationBuilder.DropColumn(
                name: "SecondGenre",
                table: "Drafts");

            migrationBuilder.DropColumn(
                name: "ThirdGenre",
                table: "Drafts");

            migrationBuilder.AddColumn<string>(
                name: "DraftId",
                table: "Genres",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_DraftId",
                table: "Genres",
                column: "DraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Drafts_DraftId",
                table: "Genres",
                column: "DraftId",
                principalTable: "Drafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
