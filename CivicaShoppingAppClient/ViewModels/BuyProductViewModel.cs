using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppClient.ViewModels
{
    public class BuyProductViewModel
    {
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
        [Required]
        public bool isAddedToCart;
    }
}
