using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Data.Implementation;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Contract;
using System;

namespace CivicaShoppingAppApi.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IProductRepository productRepository) 
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public ServiceResponse<string> PlaceOrder(int userId)
        {
            var response = new ServiceResponse<string>();

            //try
            //{
                var cartItems = _cartRepository.GetCartItemsByUserId(userId);
                var orderDate = DateTime.Now;
                var orderNumber = GenerateOrderNumber(orderDate, userId);

                foreach (var item in cartItems)
                {
                    var product = _productRepository.GetProductById(item.ProductId);
                    if (product == null)
                    {
                        response.Success = false;
                        response.Message = "Something went wrong, please try after some time.";
                        return response;
                    }
                    if (item.ProductQuantity > 5)
                    {
                        response.Success = false;
                        response.Message = "Cannot add more than 5 quantity for a product";
                        return response;
                    }
                    if (product.Quantity < item.ProductQuantity)
                    {
                        _cartRepository.RemoveParticularItem(userId, item.ProductId);
                        continue;
                    }

                    Order order = new Order()
                    {
                        OrderNumber = orderNumber,
                        UserId = userId,
                        ProductId = item.ProductId,
                        OrderDate = orderDate,
                        OrderQuantity = item.ProductQuantity,
                        OrderAmount = item.Product.finalPrice * item.ProductQuantity,
                    };

                    var result = _orderRepository.PlaceOrder(order);

                    if (result)
                    {
                        product.Quantity = product.Quantity - item.ProductQuantity;
                        _cartRepository.RemoveParticularItem(userId, item.ProductId);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Failed to place order for product " + product.ProductName;
                        return response;
                    }


                    response.Success = true;
                    response.Message = "Order placed successfully!";
                    response.Data = orderNumber.ToString();

                }
            //}
            //catch (Exception ex)
            //{
            //    response.Success = false;
            //    response.Message = "An error occurred: " + ex.Message;
            //}
            

            return response;
        }

        public ServiceResponse<IEnumerable<OrderDto>> GetOrderByOrderNumber(int orderNumber)
        {
            var response = new ServiceResponse<IEnumerable<OrderDto>>();

            var orderItems = _orderRepository.GetOrderByOrderNumber(orderNumber);
            if (orderItems != null && orderItems.Any())
            {
                List<OrderDto> orderDtos = new List<OrderDto>();
                
                double totalAmount = 0;
                foreach(var item in orderItems)
                {
                    totalAmount += item.OrderAmount;
                }

                foreach (var item in orderItems)
                {
                    OrderDto orderDto = new OrderDto();

                    orderDto.OrderId = item.OrderId;
                    orderDto.UserId = item.UserId;
                    orderDto.OrderNumber = item.OrderNumber;
                    orderDto.OrderDate = item.OrderDate;
                    orderDto.OrderQuantity = item.OrderQuantity;
                    orderDto.ProductId = item.ProductId;
                    orderDto.OrderAmount = totalAmount;
                    orderDto.Product = new Product()
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.ProductName,
                        ProductDescription = item.Product.ProductDescription,
                        Quantity = item.Product.Quantity,
                        ProductPrice = item.Product.ProductPrice,
                        GstPercentage = item.Product.GstPercentage,
                        finalPrice = item.Product.finalPrice,
                    };

                    orderDtos.Add(orderDto);
                }

                response.Success = true;
                response.Data = orderDtos;
                response.Message = "Order details found";
            }
            else
            {
                response.Success = false;
                response.Message = "Order not found";
            }

            return response;
        }

        public ServiceResponse<IEnumerable<OrderListDto>> GetAllOrdersByUserId(int userId,int page,int pageSize,string sort_direction)
        {
            var response = new ServiceResponse<IEnumerable<OrderListDto>>();

            var orderItems = _orderRepository.GetAllOrdersByUserId(userId,page,pageSize,sort_direction);
            if (orderItems != null && orderItems.Any())
            {
                response.Success = true;
                response.Data = orderItems;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found";
            }

            return response;
        }

        public ServiceResponse<int> TotalOrderByUser(int userId)
        {
            var response = new ServiceResponse<int>();
            int count = _orderRepository.TotalOrderByUserId(userId);

            response.Data = count;
            response.Success = true;

            return response;
        }



        private int GenerateOrderNumber(DateTime orderDate, int userId)
        {
            Random random = new Random();
            // Generate a random number between 1000 and 9999
            int randomPart = random.Next(1000, 9999);

            // Combine userId, orderDate.Ticks, and randomPart to generate a unique order number
            long ticks = orderDate.Ticks;
            long combined = (userId * 10000000000) + (ticks % 10000000000) + randomPart;

            // Use modulo to limit to the range of an int
            int orderNumber = (int)(combined % int.MaxValue);

            return orderNumber;
        }
    }
}
