using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppClient.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is requried")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
