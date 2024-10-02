using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CivicaShoppingAppApi.Migrations
{
    public partial class RemoveUniqueConstraintFromOrderNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);
        }
    }
}
