using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace project.Data.Migrations
{
    public partial class identit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    Invoice_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    item_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoices", x => x.Invoice_Id);
                    table.ForeignKey(
                        name: "FK_invoices_items_item_Id",
                        column: x => x.item_Id,
                        principalTable: "items",
                        principalColumn: "item_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_invoices_item_Id",
                table: "invoices",
                column: "item_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoices");
        }
    }
}
