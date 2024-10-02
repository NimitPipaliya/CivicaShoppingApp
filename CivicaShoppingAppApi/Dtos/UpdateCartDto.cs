using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Dtos
{
    public class UpdateCartDto
    {
        [Required]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductQuantity { get; set; }
    }
}
