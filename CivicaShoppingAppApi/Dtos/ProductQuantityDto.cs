using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Dtos
{
    public class ProductQuantityDto
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
