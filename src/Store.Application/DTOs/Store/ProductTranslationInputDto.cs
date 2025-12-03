using System;
using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs.Store;

public class ProductTranslationInputDto
{
 [Required]
 public string Language { get; set; } = default!;

 [Required]
 public string Name { get; set; } = default!;

 public string Description { get; set; }
}
