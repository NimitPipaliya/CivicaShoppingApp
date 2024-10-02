using CivicaShoppingAppApi.Models;
using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Dtos
{
    public class UserCartDto
    {
        [Required]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductQuantity { get; set; }

        public Product Product { get; set; }
    }
}
