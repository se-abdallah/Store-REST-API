using System;

namespace Store.Application.Params;

public class ProductParams : BaseParams
{
 public string Search { get; set; }
 public string Language { get; set; } = "en";
 public bool InStockOnly { get; set; } = false;
}
