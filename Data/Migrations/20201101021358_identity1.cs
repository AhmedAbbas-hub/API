using Microsoft.EntityFrameworkCore.Migrations;

namespace project.Data.Migrations
{
    public partial class identity1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_path",
                table: "imges");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "imges",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "imges");

            migrationBuilder.AddColumn<int>(
                name: "image_path",
                table: "imges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
