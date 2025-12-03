using System;
using Microsoft.AspNetCore.Identity;

namespace Store.Infrastructure.Identity;

public class AppUser : IdentityUser<int>
{
 public string FullName { get; set; } = default!;
 public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}