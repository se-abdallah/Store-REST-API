using System;
using Microsoft.AspNetCore.Identity;
using Store.Infrastructure.Identity;

namespace Store.Infrastructure.Data;

public class IdentitySeed
{
 public static async Task SeedAsync(
             UserManager<AppUser> userManager,
             RoleManager<AppRole> roleManager)
 {
  var roles = new List<string>
            {
                RoleNames.Admin,
                RoleNames.Visitor
            };

  foreach (var roleName in roles)
  {
   if (!await roleManager.RoleExistsAsync(roleName))
   {
    await roleManager.CreateAsync(new AppRole { Name = roleName });
   }
  }

  var adminEmail = "admin@store.com";
  var adminUserName = "admin";

  var admin = await userManager.FindByEmailAsync(adminEmail);
  if (admin == null)
  {
   admin = new AppUser
   {
    UserName = adminUserName,
    Email = adminEmail,
    FullName = "Administrator"
   };

   await userManager.CreateAsync(admin, "Admin#123");
   await userManager.AddToRoleAsync(admin, RoleNames.Admin);
  }

  var visitorEmail = "visitor@store.com";
  var visitorUserName = "visitor";

  var visitor = await userManager.FindByEmailAsync(visitorEmail);
  if (visitor == null)
  {
   visitor = new AppUser
   {
    UserName = visitorUserName,
    Email = visitorEmail,
    FullName = "Store Visitor"
   };

   await userManager.CreateAsync(visitor, "Visitor#123");
   await userManager.AddToRoleAsync(visitor, RoleNames.Visitor);
  }
 }
}
