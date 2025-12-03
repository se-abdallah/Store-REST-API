using System;

namespace Store.Application.DTOs.Store;

public class InvoiceDto
{
 public int Id { get; set; }
 public DateTime CreatedAtUtc { get; set; }
 public decimal TotalAmount { get; set; }
 public int TotalProducts { get; set; }
 public int TotalQuantity { get; set; }
 public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
}
