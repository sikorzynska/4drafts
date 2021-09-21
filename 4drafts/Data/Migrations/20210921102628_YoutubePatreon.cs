using Microsoft.EntityFrameworkCore.Migrations;

namespace _4drafts.Data.Migrations
{
    public partial class YoutubePatreon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discord",
                table: "AspNetUsers",
                newName: "Youtube");

            migrationBuilder.AddColumn<string>(
                name: "Patreon",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Patreon",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Youtube",
                table: "AspNetUsers",
                newName: "Discord");
        }
    }
}
