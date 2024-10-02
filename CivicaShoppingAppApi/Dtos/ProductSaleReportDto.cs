using CivicaShoppingAppApi.Models;

namespace CivicaShoppingAppApi.Dtos
{
    public class ProductSaleReportDto
    {
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalQuantitySold {  get; set; }
        //public Product ProductSaleProduct { get; set; }
        public string ProductName { get; set; } 
    }
}
