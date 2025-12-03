using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs.Store;

public class CreateInvoiceDto
{
 [MinLength(1, ErrorMessage = "At least one item is required.")]
 public List<CreateInvoiceItemDto> Items { get; set; } = new List<CreateInvoiceItemDto>();
}
