using Microsoft.EntityFrameworkCore.Migrations;

namespace _4drafts.Data.Migrations
{
    public partial class ManyGenres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Genres_GenreId",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Threads_GenreId",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Threads");

            migrationBuilder.CreateTable(
                name: "GenreThreads",
                columns: table => new
                {
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    ThreadId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreThreads", x => new { x.GenreId, x.ThreadId });
                    table.ForeignKey(
                        name: "FK_GenreThreads_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreThreads_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenreThreads_ThreadId",
                table: "GenreThreads",
                column: "ThreadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenreThreads");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Threads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Threads_GenreId",
                table: "Threads",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_Genres_GenreId",
                table: "Threads",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
