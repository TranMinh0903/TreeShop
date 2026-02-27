
namespace PRN232.LaptopShop.Services.Response
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        //public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
