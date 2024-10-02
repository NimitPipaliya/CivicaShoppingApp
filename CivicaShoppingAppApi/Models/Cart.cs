using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductQuantity { get; set; }

        public User User { get; set; }
        public Product Product { get; set; }
    }
}
