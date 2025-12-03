namespace Store.Domain.Entities;

public class BasketItem : BaseEntity
{
 public int BasketId { get; set; }
 public Basket Basket { get; set; } = default!;

 public int ProductId { get; set; }
 public Product Product { get; set; } = default!;

 public int Quantity { get; set; }
}