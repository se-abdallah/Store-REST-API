using System;

namespace Store.Application.DTOs.Store;

public class ProductPublicDetailDto
{
 public int Id { get; set; }

 public decimal Price { get; set; }

 public int StockQuantity { get; set; }

 public bool IsInStock { get; set; }

 public string Name { get; set; } = default!;

 public string Description { get; set; }
}
