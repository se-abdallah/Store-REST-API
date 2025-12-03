using System;
using System.Collections.Generic;

namespace Store.Application.DTOs.Store;

public class ProductAdminDetailDto
{
 public int Id { get; set; }

 public decimal Price { get; set; }

 public int StockQuantity { get; set; }

 public bool IsInStock { get; set; }
 public bool IsDeleted { get; set; }

 public List<ProductTranslationDto> Translations { get; set; } = new();
}
