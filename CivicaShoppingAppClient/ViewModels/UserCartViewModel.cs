using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppClient.ViewModels
{
    public class UserCartViewModel
    {
        [Required]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductQuantity { get; set; }

        public ProductListViewModel Product { get; set; }
    }
}
