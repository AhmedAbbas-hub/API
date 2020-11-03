using Microsoft.EntityFrameworkCore.Migrations;

namespace project.Data.Migrations
{
    public partial class identity2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_imges_items_itemsitem_Id",
                table: "imges");

            migrationBuilder.DropIndex(
                name: "IX_imges_itemsitem_Id",
                table: "imges");

            migrationBuilder.DropColumn(
                name: "itemsitem_Id",
                table: "imges");

            migrationBuilder.CreateIndex(
                name: "IX_imges_item_Id",
                table: "imges",
                column: "item_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_imges_items_item_Id",
                table: "imges",
                column: "item_Id",
                principalTable: "items",
                principalColumn: "item_Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_imges_items_item_Id",
                table: "imges");

            migrationBuilder.DropIndex(
                name: "IX_imges_item_Id",
                table: "imges");

            migrationBuilder.AddColumn<int>(
                name: "itemsitem_Id",
                table: "imges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_imges_itemsitem_Id",
                table: "imges",
                column: "itemsitem_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_imges_items_itemsitem_Id",
                table: "imges",
                column: "itemsitem_Id",
                principalTable: "items",
                principalColumn: "item_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
