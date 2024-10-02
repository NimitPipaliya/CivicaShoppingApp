using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CivicaShoppingAppApi.Data.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly IAppDbContext _context;

        public ProductRepository(IAppDbContext appDbcontext)
        {
            _context = appDbcontext;
        }


        //--------------Get all products with pagination----------------
        public IEnumerable<Product> GetPaginatedProducts(int page, int pageSize, string sort_direction)
        {
            int skip = (page - 1) * pageSize;
            if (sort_direction == "desc")
            {
                return _context.Products.OrderByDescending(c => c.ProductName).Skip(skip)
                 .Take(pageSize)
                 .ToList();
            }
            else
            {
                return _context.Products.OrderBy(c => c.ProductName)
                    .Skip(skip)
                .Take(pageSize)
                .ToList();
            }
        }

        //--------------Add Product----------------
        public bool InsertProduct(Product product)
        {
            var result = false;
            if (product != null)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }

        //--------------Get product by id----------------
        public Product GetProductById(int id)
        {
            var products = _context.Products.FirstOrDefault(c => c.ProductId == id);
            return products;
        }

        //--------------Update product----------------
        public bool UpdateProduct(Product product)
        {
            var result = false;
            if (product != null)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }

        //--------------Delete product----------------
        public bool DeleteProduct(int id)
        {
            var result = false;
            if (id > 0)
            {
                var product = _context.Products.Find(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        //--------------Product Exist ----------------
        public bool ProductExists(string productName)
        {
            var productExist = _context.Products.FirstOrDefault(c => c.ProductName.ToLower() == productName.ToLower());
            if (productExist != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //--------------Product Exist with id----------------
        public bool ProductExists(int productId, string productName)
        {
            var productExist = _context.Products.FirstOrDefault(c => c.ProductId != productId && c.ProductName.ToLower() == productName.ToLower());
            if (productExist != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //--------------Total products for pagination----------------
        public int TotalProducts()
        {
            return _context.Products.Count();
        }

        //------------------Searching and sorting----------------
        [ExcludeFromCodeCoverage]
        public IEnumerable<Product> GetPaginatedProductsWithFilter(string? character, int page, int pageSize, string sort_direction)
        {
            var products = _context.Products.AsQueryable();

            if (character != null)
            {
                products = products.Where(c => c.ProductName.StartsWith(character.ToLower()));
            }

            if (sort_direction == "desc")
            {
                products = products.OrderByDescending(c => c.ProductName);
            }
            else
            {
                products = products.OrderBy(c => c.ProductName);
            }

            int skip = (page - 1) * pageSize;

            return products
                .Skip(skip)
                .Take(pageSize)
                .ToList();
        }

        //-------------------total products with search--------------
        [ExcludeFromCodeCoverage]
        public int TotalProductsStartingWithLetter(string? ch)
        {
            var Products = _context.Products.AsQueryable();
            if (ch != null)
            {
                var ProductsCount = Products.Where(c => c.ProductName.StartsWith(ch.ToLower())).Count();
                return ProductsCount;
            }
            else
            {
                return _context.Products.Count();
            }
        }
        
        //--------------------available quantity of specifiv product------------------

       
        public IEnumerable<Product> GetQuantityOfSpecificProducts(int page, int pageSize, string sortOrder)
        {
            int skip = (page - 1) * pageSize;
            IQueryable<Product> query = _context.Products;

            switch (sortOrder.ToLower())
            {
                case "asc":
                    query = query.OrderBy(c => c.ProductName);
                    break;
                case "desc":
                    query = query.OrderByDescending(c => c.ProductName);
                    break;
                default:
                    query = query.OrderBy(c => c.ProductName);
                    break;
            }

            return query
                .Skip(skip)
                .Take(pageSize)
                .ToList();
        }
        public IEnumerable<ProductSaleReportDto> GetProductSalesReport(int page, int pageSize, string sortOrder)
        {
            int skip = (page - 1) * pageSize;
            var query = _context.Orders.Include(c => c.User)
                .GroupBy(o => new
                {
                    o.ProductId,
                    OrderDate = new DateTime(o.OrderDate.Year, o.OrderDate.Month, o.OrderDate.Day)  // Truncate time part
                })
                .Select(g => new ProductSaleReportDto
                {
                    ProductId = g.Key.ProductId,
                    OrderDate = g.Key.OrderDate,
                    TotalQuantitySold = g.Sum(o => o.OrderQuantity),
                    ProductName = g.Select(o => o.Product.ProductName).FirstOrDefault(),
                });
            switch (sortOrder.ToLower())
            {
                case "asc":
                    query = query.OrderBy(c => c.ProductName);
                    break;
                case "desc":
                    query = query.OrderByDescending(c => c.ProductName);
                    break;
                default:
                    query = query.OrderBy(c => c.ProductName);
                    break;
            }
            return query
                .Skip(skip)
                .Take(pageSize)
                .ToList();  // Execute query and return results as a list
        }


        public int ProductsSoldCount()
        {
            var query = _context.Orders.Include(c => c.User)
             .GroupBy(o => new
             {
                 o.ProductId,
                 OrderDate = new DateTime(o.OrderDate.Year, o.OrderDate.Month, o.OrderDate.Day)  // Truncate time part
             })
             .Select(g => new ProductSaleReportDto
             {
                 ProductId = g.Key.ProductId,
                 OrderDate = g.Key.OrderDate,
                 TotalQuantitySold = g.Sum(o => o.OrderQuantity),
                 ProductName = g.Select(o => o.Product.ProductName).FirstOrDefault(),
             }).ToList();

            return query.Count();
        }


    }
}
