using System;

namespace Store.Application.DTOs.Store;

public class InvoiceItemDto
{
 public int ProductId { get; set; }

 public string ProductName { get; set; }

 public string ProductDescription { get; set; }

 public decimal UnitPrice { get; set; }

 public int Quantity { get; set; }

 public decimal LineTotal { get; set; }
}
