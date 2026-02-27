namespace PRN232.LaptopShop.Services.Request
{
    public class ProductRequest
    {
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int? StockQuantity { get; set; }
        public int CategoryId { get; set; }
    }
}
