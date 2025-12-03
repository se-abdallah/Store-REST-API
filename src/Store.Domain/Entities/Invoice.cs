namespace Store.Domain.Entities;

public class Invoice : BaseEntity
{
 public DateTime CreatedAtUtc { get; set; }

 public int UserId { get; set; }

 public decimal TotalAmount { get; set; }

 public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
}
