using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CivicaShoppingAppApi.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Salutation { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        [Required]
        [StringLength(15)]
        public string LoginId { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(12)]
        public string Phone { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; } 
        [Required]
        public int SecurityQuestionId { get; set; }
        [Required]
        [StringLength(15)]
        public string Answer {  get; set; }

        public bool IsAdmin { get; set; } = false;
        public SecurityQuestion SecurityQuestion { get; set; }
        
        public ICollection<Order> Orders { get; set; }
        public ICollection<Cart> Carts { get; set; }
    }
}
