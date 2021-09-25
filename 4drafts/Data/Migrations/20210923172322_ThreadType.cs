using Microsoft.EntityFrameworkCore.Migrations;

namespace _4drafts.Data.Migrations
{
    public partial class ThreadType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Drafts");

            migrationBuilder.AddColumn<string>(
                name: "PromptId",
                table: "Threads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThreadTypeId",
                table: "Threads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DraftId",
                table: "Genres",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThreadTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SimplifiedName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Threads_ThreadTypeId",
                table: "Threads",
                column: "ThreadTypeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_ThreadTypes_ThreadTypeId",
                table: "Threads",
                column: "ThreadTypeId",
                principalTable: "ThreadTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Drafts_DraftId",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_ThreadTypes_ThreadTypeId",
                table: "Threads");

            migrationBuilder.DropTable(
                name: "ThreadTypes");

            migrationBuilder.DropIndex(
                name: "IX_Threads_ThreadTypeId",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Genres_DraftId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "PromptId",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "ThreadTypeId",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "DraftId",
                table: "Genres");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Threads",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Drafts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
