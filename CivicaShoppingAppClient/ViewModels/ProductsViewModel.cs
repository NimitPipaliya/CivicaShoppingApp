namespace CivicaShoppingAppClient.ViewModels
{
    public class ProductsViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int Quantity { get; set; }
        public double ProductPrice { get; set; }
        public double GstPercentage { get; set; }
        public double finalPrice { get; set; }
    }
}
