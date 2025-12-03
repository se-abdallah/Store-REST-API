using System;
using System.Security.Claims;

namespace Store.API.Extensions;

public static class ClaimsPrincipalExtensions
{
 public static string GetUsername(this ClaimsPrincipal user)
 {
  var username = user.FindFirstValue(ClaimTypes.Name);
  if (string.IsNullOrEmpty(username))
  {
   throw new Exception("Cannot get username from token.");
  }

  return username;
 }

 public static int GetUserId(this ClaimsPrincipal user)
 {
  var idValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
  if (string.IsNullOrEmpty(idValue))
  {
   throw new Exception("Cannot get user id from token.");
  }

  return int.Parse(idValue);
 }

 public static IEnumerable<string> GetRolesList(this ClaimsPrincipal user)
 {
  return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
 }

 public static string GetRolesString(this ClaimsPrincipal user)
 {
  return string.Join(",", user.GetRolesList());
 }
}
