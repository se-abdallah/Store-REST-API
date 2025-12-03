using System;

namespace Store.Application.DTOs.Store;

public class ProductListItemDto
{
 public int Id { get; set; }

 public string Name { get; set; } = default!;

 public decimal Price { get; set; }

 public bool IsInStock { get; set; }
 public int StockQuantity { get; set; }
}
