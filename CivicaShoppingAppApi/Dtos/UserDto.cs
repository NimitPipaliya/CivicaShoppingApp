using CivicaShoppingAppApi.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Dtos
{
    public class UserDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [DisplayName("Salutation")]
        public string Salutation { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [DisplayName("Date of Birth")]
        public DateTime BirthDate { get; set; }

        [Required]
        public int Age { get; set; }

        [Required(ErrorMessage = "Login Id is required")]
        [StringLength(15)]
        public string LoginId { get; set; }


        [Required]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress]
        [StringLength(50)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(15)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$", ErrorMessage = "Invalid phone number.")]
        [DisplayName("Phone Number")]
        public string Phone { get; set; }
 

 

    }
}
