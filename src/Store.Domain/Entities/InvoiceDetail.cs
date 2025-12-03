namespace Store.Domain.Entities;

public class InvoiceDetail : BaseEntity
{
 public int InvoiceId { get; set; }
 public Invoice Invoice { get; set; } = default!;
 public int ProductId { get; set; }
 public Product Product { get; set; } = default!;
 public string ProductName { get; set; } = string.Empty;
 public string? ProductDescription { get; set; }
 public decimal UnitPrice { get; set; }
 public int Quantity { get; set; }
 public decimal LineTotal { get; set; }
}
