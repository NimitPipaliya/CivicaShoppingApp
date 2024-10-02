using System.ComponentModel.DataAnnotations;

namespace CivicaShoppingAppApi.Models
{
    public class SecurityQuestion
    {
        [Key]
        public int SecurityQuestionId { get; set; }
        public string Question { get; set; }
   
    }
}
