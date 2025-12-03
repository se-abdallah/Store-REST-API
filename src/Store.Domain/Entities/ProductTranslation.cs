namespace Store.Domain.Entities;

public class ProductTranslation : BaseEntity
{
 public int ProductId { get; set; }
 public Product Product { get; set; } = default!;
 public string Language { get; set; } = default!;
 public string Name { get; set; } = default!;
 public string? Description { get; set; }
}
