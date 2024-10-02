using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicaShoppingAppApi.Data.Contract
{
    public interface IProductRepository
    {
        bool InsertProduct(Product product);
        //  IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetPaginatedProducts(int page, int pageSize,string sort_direction);
        Product GetProductById(int id);
        bool UpdateProduct(Product product);
        bool DeleteProduct(int id);
        bool ProductExists(string productName);

        bool ProductExists(int productId, string productName);

        int TotalProducts();
        IEnumerable<Product> GetPaginatedProductsWithFilter(string? character, int page, int pageSize, string sort_direction);
        //-------------------total products with search--------------
        int TotalProductsStartingWithLetter(string? ch);
        IEnumerable<Product> GetQuantityOfSpecificProducts(int page, int pageSize, string sortOrder);
        IEnumerable<ProductSaleReportDto> GetProductSalesReport(int page, int pageSize, string sortOrder);

        int ProductsSoldCount();
    }
}
