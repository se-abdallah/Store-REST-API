using System;
using Microsoft.AspNetCore.Identity;

namespace Store.Infrastructure.Identity;

public class AppRole : IdentityRole<int>
{
 public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}
