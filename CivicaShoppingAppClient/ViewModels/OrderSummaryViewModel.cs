
namespace CivicaShoppingAppClient.ViewModels
{
    public class OrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderQuantity { get; set; }
        public double OrderAmount { get; set; }
        public UserViewModel User { get; set; }
        public ProductsViewModel Product { get; set; }
    }
}
