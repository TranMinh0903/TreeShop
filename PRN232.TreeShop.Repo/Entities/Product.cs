using System;
using System.Collections.Generic;

namespace PRN232.LaptopShop.Repo.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int? StockQuantity { get; set; }

    public bool? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;
}
