namespace Store.Domain.Entities;

public class Product : BaseEntity
{
 public decimal Price { get; set; }
 public bool IsDeleted { get; set; }
 public int StockQuantity { get; set; }
 public bool IsInStock => StockQuantity > 0;
 public ICollection<ProductTranslation> Translations { get; set; } = new List<ProductTranslation>();
}