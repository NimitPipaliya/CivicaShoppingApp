using CivicaShoppingAppApi.Dtos;

namespace CivicaShoppingAppApi.Services.Contract
{
    public interface IProductService
    {
        ServiceResponse<IEnumerable<ProductListDto>> GetPaginatedProducts(int page, int pageSize, string sort_direction);
        ServiceResponse<int> TotalProduct();
        ServiceResponse<ProductListDto> GetProduct(int id);
        ServiceResponse<string> AddProduct(AddProductDto productDataDto);
        ServiceResponse<string> ModifyProduct(UpdateProductDto productDataDto);
        ServiceResponse<string> RemoveProduct(int id);
        ServiceResponse<IEnumerable<ProductListDto>> GetPaginatedProductsWithSearch(string search, int page, int pageSize, string sort_dir);
        ServiceResponse<int> TotalProductStartingWithString(string? searchString);
        ServiceResponse<IEnumerable<ProductQuantityDto>> GetQuantityOfSpecificProduct(int page, int pageSize, string sortOrder);
        ServiceResponse<IEnumerable<ProductSaleReportDto>> GetProductSalesReport(int page, int pageSize, string sortOrder);

        ServiceResponse<int> ProductsSoldCount();
    }
}
