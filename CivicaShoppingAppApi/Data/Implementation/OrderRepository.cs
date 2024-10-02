using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicaShoppingAppApi.Data.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IAppDbContext _context;

        public OrderRepository(IAppDbContext appDbcontext)
        {
            _context = appDbcontext;
        }

        public IEnumerable<Order> GetOrderByOrderNumber(int orderNumber)
        {
            var order = _context.Orders.Include(c => c.Product).Where(c => c.OrderNumber == orderNumber);

            return order.ToList();
        }

        public IEnumerable<OrderListDto> GetAllOrdersByUserId(int userId,int page,int pageSize,string sort_direction)
        {
            int skip = (page - 1) * pageSize;

            var query = _context.Orders.Where(c => c.UserId == userId)
                .GroupBy(o => new
                {
                    o.OrderNumber,
                    OrderDate = new DateTime(o.OrderDate.Year, o.OrderDate.Month, o.OrderDate.Day),
                })
                .Select(g => new OrderListDto
                {
                    OrderNumber = g.Key.OrderNumber,
                    OrderDate = g.Key.OrderDate,
                }).AsEnumerable();

            if(sort_direction == "desc")
            {
                query = query.OrderByDescending(c => c.OrderDate);
            }
            else
            {
                query = query.OrderBy(c => c.OrderDate);
            }

            return query
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();
        }

        public int TotalOrderByUserId(int userId)
        {
            return _context.Orders.Where(c => c.UserId == userId)
                .GroupBy(o => new
                {
                    o.OrderNumber,
                    OrderDate = new DateTime(o.OrderDate.Year, o.OrderDate.Month, o.OrderDate.Day),
                }).Count();
        }

        public bool PlaceOrder(Order order)
        {
            var result = false;
            if (order != null)
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }
    }
}
