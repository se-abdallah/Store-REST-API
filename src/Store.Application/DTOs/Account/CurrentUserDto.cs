using System;
using System.Collections.Generic;

namespace Store.Application.DTOs.Account;

public class CurrentUserDto
{
 public int Id { get; set; }

 public string UserName { get; set; }

 public string Email { get; set; }

 public string FullName { get; set; }

 public IEnumerable<string> Roles { get; set; }
}
