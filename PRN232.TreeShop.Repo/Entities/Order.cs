using System;
using System.Collections.Generic;

namespace PRN232.LaptopShop.Repo.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? ShipperId { get; set; }

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

    public virtual Account User { get; set; } = null!;

    public virtual Account? Shipper { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
