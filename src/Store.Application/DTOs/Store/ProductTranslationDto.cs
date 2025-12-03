using System;

namespace Store.Application.DTOs.Store;

public class ProductTranslationDto
{
 public string Language { get; set; } = default!;
 public string Name { get; set; } = default!;
 public string Description { get; set; }
}
