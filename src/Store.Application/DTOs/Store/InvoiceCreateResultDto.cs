using System;

namespace Store.Application.DTOs.Store;

public class InvoiceCreateResultDto
{
 public bool Success { get; set; }

 public InvoiceDto Invoice { get; set; }

 public List<string> Errors { get; set; } = new List<string>();
}
