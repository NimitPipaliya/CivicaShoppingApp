using CivicaShoppingAppApi.Models;
using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Dtos
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderQuantity { get; set; }
        public double OrderAmount { get; set; }

        public User User { get; set; }
        public Product Product { get; set; }
    }
}
