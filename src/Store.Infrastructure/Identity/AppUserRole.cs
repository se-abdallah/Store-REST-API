using System;
using Microsoft.AspNetCore.Identity;

namespace Store.Infrastructure.Identity;

public class AppUserRole : IdentityUserRole<int>
{
 public AppUser User { get; set; } = default!;
 public AppRole Role { get; set; } = default!;
}