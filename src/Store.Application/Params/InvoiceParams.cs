using System;

namespace Store.Application.Params;

public class InvoiceParams : BaseParams
{
 public DateTime? From { get; set; }
 public DateTime? To { get; set; }
 public string UserEmail { get; set; }
 public string OrderBy { get; set; } = "date_desc";
}
