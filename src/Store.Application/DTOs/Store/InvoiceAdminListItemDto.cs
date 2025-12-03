using System;

namespace Store.Application.DTOs.Store;

public class InvoiceAdminListItemDto
{
 public int Id { get; set; }

 public DateTime CreatedAtUtc { get; set; }

 public decimal TotalAmount { get; set; }

 public int TotalProducts { get; set; }

 public int TotalQuantity { get; set; }

 public int UserId { get; set; }

 public string UserEmail { get; set; }

 public string UserFullName { get; set; }
}
