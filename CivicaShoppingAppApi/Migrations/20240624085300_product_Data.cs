using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CivicaShoppingAppApi.Migrations
{
    public partial class product_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "Products",
              columns: new[] { "ProductId", "ProductName", "ProductDescription", "Quantity", "ProductPrice", "GstPercentage", "finalPrice" },
              values: new object[,]
              {
                    { 1,"Product 1","Description 1",10,100.0,18,118.0 }

              }

              );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
             table: "Products",
             keyColumn: "ProductId",
             keyValue: new object[] { 1}
             );
        }
    }
}
