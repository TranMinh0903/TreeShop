namespace PRN232.LaptopShop.Services.Request
{
    public class OrderRequest
    {
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string? ShippingMethod { get; set; } // "Standard" or "Express"
        public string? PaymentMethod { get; set; } // "COD" or "BankTransfer"
        public string? Note { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
