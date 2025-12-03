using System;
using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs.Account;

public class LoginDto
{
 [Required(ErrorMessage = "Email or Username must be provided.")]
 public string Identifier { get; set; }

 [Required(ErrorMessage = "Password must be provided.")]
 public string Password { get; set; }
}
