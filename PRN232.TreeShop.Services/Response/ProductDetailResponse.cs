using PRN232.LaptopShop.Services.Response;

namespace PRN232.LaptopShop.Services.Response
{
    public class ProductDetailResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int? StockQuantity { get; set; }
        //public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
