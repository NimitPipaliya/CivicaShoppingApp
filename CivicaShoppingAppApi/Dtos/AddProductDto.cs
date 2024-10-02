using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Dtos
{
    public class AddProductDto
    {
        
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ProductDescription { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]

        public double ProductPrice { get; set; }
        
    }
}
