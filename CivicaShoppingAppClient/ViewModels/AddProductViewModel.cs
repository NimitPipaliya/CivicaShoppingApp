using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppClient.ViewModels
{
    public class AddProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [MinLength(3, ErrorMessage = "Description must be at least 3 characters long.")]
     public   string ProductName { get; set; }


        [Required(ErrorMessage ="Product description is required")]
        [MinLength(5,ErrorMessage ="Description must be at least 5 characters long.")]
    public string ProductDescription { get; set; }
    
        
        [Required(ErrorMessage = "Product quantity is required")]
        [Range(0,int.MaxValue,ErrorMessage ="Value cannot be less then 0.")]
    public int Quantity {  get; set; }

        
        [Required(ErrorMessage = "Product price is required")]
        [Range(0.01, int.MaxValue, ErrorMessage = "Minimum value must be 0.01")]
        public double ProductPrice {get; set; }
    }
}
