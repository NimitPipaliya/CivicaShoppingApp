namespace CivicaShoppingAppClient.ViewModels
{
    public class CartItemViewModel
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int ProductQuantity { get; set; }
        public BuyProductViewModel Produdct { get; set; }
    }
}
