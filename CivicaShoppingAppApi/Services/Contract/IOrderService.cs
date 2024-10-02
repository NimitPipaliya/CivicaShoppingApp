using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;

namespace CivicaShoppingAppApi.Services.Contract
{
    public interface IOrderService
    {
        ServiceResponse<IEnumerable<OrderDto>> GetOrderByOrderNumber(int orderNumber);
        ServiceResponse<string> PlaceOrder(int userId);
        ServiceResponse<IEnumerable<OrderListDto>> GetAllOrdersByUserId(int userId, int page, int pageSize, string sort_direction);
        ServiceResponse<int> TotalOrderByUser(int userId);
    }
}
