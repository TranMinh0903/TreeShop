namespace PRN232.LaptopShop.Services.Request
{
    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = null!; // Confirmed, Shipping, Delivered, Completed, Cancelled
        public string? DeliveryImageUrl { get; set; } // Required when Shipper sets status to Delivered
    }
}
