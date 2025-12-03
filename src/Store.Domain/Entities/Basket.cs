namespace Store.Domain.Entities;

public class Basket : BaseEntity
{
 public int UserId { get; set; } = default!;

 public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
}
