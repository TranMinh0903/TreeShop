namespace PRN232.LaptopShop.Services.Response
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int? ShipperId { get; set; }
        public string? ShipperName { get; set; }
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string? ShippingMethod { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? DeliveryImageUrl { get; set; }
        public DateTime? DeliveryTimestamp { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderDetailResponse> OrderDetails { get; set; } = new();
    }

    public class OrderDetailResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public decimal SubTotal => Price * Quantity;
    }
}
