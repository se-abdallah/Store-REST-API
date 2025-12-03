using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs.Account;

public class UpdateProfileDto
{
 [Required(ErrorMessage = "Full name is required.")]
 public string FullName { get; set; }

 [Required(ErrorMessage = "Email is required.")]
 [EmailAddress(ErrorMessage = "Invalid email address.")]
 public string Email { get; set; }

 [Required(ErrorMessage = "Username is required.")]
 [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
 public string Username { get; set; }
}
