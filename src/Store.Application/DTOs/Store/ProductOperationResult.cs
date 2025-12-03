using System;
using System.Collections.Generic;

namespace Store.Application.DTOs.Store;

public class ProductOperationResult<T>
{
 public bool Success { get; set; }

 public T Data { get; set; }

 public List<string> Errors { get; set; } = new List<string>();
}
