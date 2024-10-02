using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;

namespace CivicaShoppingAppApi.Data.Contract
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrderByOrderNumber(int orderNumber);
        public IEnumerable<OrderListDto> GetAllOrdersByUserId(int userId, int page, int pageSize, string sort_direction);
        int TotalOrderByUserId(int userId);
        bool PlaceOrder(Order order);
    }
}
