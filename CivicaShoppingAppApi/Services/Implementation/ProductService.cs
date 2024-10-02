using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Data.Implementation;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Contract;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CivicaShoppingAppApi.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;

        }

        //------------------Get all Products with pagination----------------
        public ServiceResponse<IEnumerable<ProductListDto>> GetPaginatedProducts(int page, int pageSize, string sort_direction)
        {
            var response = new ServiceResponse<IEnumerable<ProductListDto>>();
            var allProducts = _productRepository.GetPaginatedProducts(page, pageSize, sort_direction);
            if (allProducts != null && allProducts.Any())
            {
                List<ProductListDto> productLists = new List<ProductListDto>();
                foreach (var product in allProducts)
                {

                    ProductListDto productList = new ProductListDto();
                    productList.ProductId = product.ProductId;
                    productList.ProductName = product.ProductName;
                    productList.ProductDescription = product.ProductDescription;
                    productList.Quantity = product.Quantity;
                    productList.ProductPrice = product.ProductPrice;
                    productList.GstPercentage = product.ProductPrice * 0.18;
                    productList.finalPrice = product.finalPrice;

                    productLists.Add(productList);
                }

                response.Data = productLists;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found";
            }

            return response;
        }

        public ServiceResponse<int> TotalProduct()
        {
            var response = new ServiceResponse<int>();
            int count = _productRepository.TotalProducts();

            response.Data = count;
            response.Success = true;

            return response;
        }

        //------------------Get product by id----------------

        public ServiceResponse<ProductListDto> GetProduct(int id)
        {
            var response = new ServiceResponse<ProductListDto>();
            var existingProduct = _productRepository.GetProductById(id);
            if (existingProduct != null)
            {
                var product = new ProductListDto()
                {
                    ProductId = existingProduct.ProductId,
                    ProductName = existingProduct.ProductName,
                    ProductDescription = existingProduct.ProductDescription,
                    ProductPrice = existingProduct.ProductPrice,
                    finalPrice = existingProduct.finalPrice,
                    Quantity = existingProduct.Quantity,
                    GstPercentage = existingProduct.GstPercentage,
                };
                response.Data = product;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found!";
            }
            return response;
        }

        //------------------Add Product----------------
        public ServiceResponse<string> AddProduct(AddProductDto productDataDto)
        {
            var response = new ServiceResponse<string>();

            var addProduct = new Product()
            {
                ProductName = productDataDto.ProductName,
                ProductDescription = productDataDto.ProductDescription,
                ProductPrice = productDataDto.ProductPrice,
                Quantity = productDataDto.Quantity,
                GstPercentage = productDataDto.ProductPrice * 0.18,
            };
            addProduct.finalPrice = addProduct.ProductPrice + addProduct.GstPercentage;

            if (_productRepository.ProductExists(productDataDto.ProductName))
            {
                response.Success = false;
                response.Message = "Product already exists.";
                return response;
            }

            var result = _productRepository.InsertProduct(addProduct);
            if (result)
            {
                response.Message = "Product added successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime.";
            }
            return response;
        }


        //------------------Update Product----------------
        public ServiceResponse<string> ModifyProduct(UpdateProductDto productDataDto)
        {
            var response = new ServiceResponse<string>();
            if (_productRepository.ProductExists(productDataDto.ProductId, productDataDto.ProductName))
            {
                response.Success = false;
                response.Message = "Product already exists.";
                return response;
            }
            var existingProduct = _productRepository.GetProductById(productDataDto.ProductId);
            var result = false;

            if (existingProduct != null)
            {
                existingProduct.ProductId = productDataDto.ProductId;
                existingProduct.ProductName = productDataDto.ProductName;
                existingProduct.ProductDescription = productDataDto.ProductDescription;
                existingProduct.Quantity = productDataDto.Quantity;
                existingProduct.ProductPrice = productDataDto.ProductPrice;
                existingProduct.GstPercentage = productDataDto.ProductPrice * 0.18;
            }
            existingProduct.finalPrice = existingProduct.ProductPrice + existingProduct.GstPercentage;


            result = _productRepository.UpdateProduct(existingProduct);
            if (result)
            {
                response.Message = "Product updated successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime.";
            }
            return response;
        }

        //------------------Delete Product----------------
        public ServiceResponse<string> RemoveProduct(int id)
        {
            var response = new ServiceResponse<string>();
            var result = _productRepository.DeleteProduct(id);
            if (result)
            {

                response.Success = true;
                response.Message = "Product deleted successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime.";
            }
            return response;
        }

        //---------------------------Searching and sorting---------------------
        public ServiceResponse<IEnumerable<ProductListDto>> GetPaginatedProductsWithSearch(string search, int page, int pageSize, string sort_dir)
        {
            var response = new ServiceResponse<IEnumerable<ProductListDto>>();
            var allSearchedProducts = _productRepository.GetPaginatedProductsWithFilter(search, page, pageSize, sort_dir);
            if (allSearchedProducts != null && allSearchedProducts.Any())
            {
                List<ProductListDto> productListDtos = new List<ProductListDto>();
                foreach (var product in allSearchedProducts)
                {
                    ProductListDto productDto = new ProductListDto();

                    productDto.ProductId = product.ProductId;
                    productDto.ProductName = product.ProductName;
                    productDto.ProductDescription = product.ProductDescription;
                    productDto.ProductPrice = product.ProductPrice;
                    productDto.finalPrice = product.finalPrice;
                    productDto.Quantity = product.Quantity;
                    productDto.GstPercentage = product.GstPercentage;
                    productListDtos.Add(productDto);
                }

                response.Data = productListDtos;
                response.Success = true;
                response.Message = "Success";
            }
            else
            {
                response.Success = false;
                response.Message = "No record found";
            }

            return response;
        }

        public ServiceResponse<int> TotalProductStartingWithString(string? searchString)
        {
            var response = new ServiceResponse<int>();
            int count = _productRepository.TotalProductsStartingWithLetter(searchString);

            response.Data = count;
            response.Success = true;

            return response;
        }


        public ServiceResponse<IEnumerable<ProductQuantityDto>> GetQuantityOfSpecificProduct(int page, int pageSize, string sortOrder)
        {
            var response = new ServiceResponse<IEnumerable<ProductQuantityDto>>();
            var products = _productRepository.GetQuantityOfSpecificProducts(page, pageSize, sortOrder);

            if (products != null && products.Any())
            {
                List<ProductQuantityDto> productsDto = new List<ProductQuantityDto>();
                foreach (var product in products.ToList())
                {
                    productsDto.Add(new ProductQuantityDto()
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Quantity = product.Quantity,

                    });
                }


                response.Data = productsDto;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found";
            }

            return response;
        }
        public ServiceResponse<IEnumerable<ProductSaleReportDto>> GetProductSalesReport(int page, int pageSize, string sortOrder)
        {
            var response = new ServiceResponse<IEnumerable<ProductSaleReportDto>>();
            var products = _productRepository.GetProductSalesReport(page, pageSize, sortOrder);

            if (products != null && products.Any())
            {
                List<ProductSaleReportDto> productsDto = new List<ProductSaleReportDto>();
                foreach (var product in products.ToList())
                {
                    productsDto.Add(new ProductSaleReportDto()
                    {
                        ProductId = product.ProductId,
                        OrderDate = product.OrderDate,
                        TotalQuantitySold = product.TotalQuantitySold,
                        ProductName = product.ProductName
                    });
                }
                response.Data = productsDto;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found";
            }

            return response;
        }

        public ServiceResponse<int> ProductsSoldCount()
        {
            var response = new ServiceResponse<int>();
            int totalProductsSold = _productRepository.ProductsSoldCount();

            response.Data = totalProductsSold;
            return response;
        }
    }
}   

