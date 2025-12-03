using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs.Store;

public class UpdateProductDto
{
 [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
 public decimal Price { get; set; }

 [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
 public int StockQuantity { get; set; }

 [MinLength(1, ErrorMessage = "At least one translation is required.")]
 public List<ProductTranslationInputDto> Translations { get; set; } = new();
}
