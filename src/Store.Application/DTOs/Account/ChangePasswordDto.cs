using System;
using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs.Account;

public class ChangePasswordDto
{
 [Required(ErrorMessage = "Current password is required.")]
 public string CurrentPassword { get; set; }

 [Required(ErrorMessage = "New password is required.")]
 [MinLength(6, ErrorMessage = "New password must contain at least 6 characters.")]
 public string NewPassword { get; set; }
}
