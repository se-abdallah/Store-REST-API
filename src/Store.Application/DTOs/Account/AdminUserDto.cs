using System;

namespace Store.Application.DTOs.Account;

public class AdminUserDto
{
 public int Id { get; set; }

 public string Email { get; set; }

 public string Username { get; set; }

 public string FullName { get; set; }

 public IEnumerable<string> Roles { get; set; }

 public bool IsLockedOut { get; set; }
}
