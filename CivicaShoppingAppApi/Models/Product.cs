using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
            
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ProductDescription { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]

        public double ProductPrice { get; set; }
        [Required]
        public double GstPercentage { get; set; }
        [Required]
        public double finalPrice { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<Cart> Carts { get; set; }
    }
}
