using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Models
{
    public class Order
    {
        [Key]
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
