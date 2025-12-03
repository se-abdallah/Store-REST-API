using System;
using Microsoft.AspNetCore.Identity;
using Store.Application.DTOs;
using Store.Application.DTOs.Account;
using Store.Application.Interfaces;
using Store.Infrastructure.Identity;

namespace Store.Infrastructure.Implementation;

public class UserAdminService : IUserAdminService
{
 private readonly UserManager<AppUser> _userManager;
 private readonly RoleManager<AppRole> _roleManager;

 public UserAdminService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
 {
  _userManager = userManager;
  _roleManager = roleManager;
 }

 public async Task<IReadOnlyList<AdminUserDto>> GetAllUsersAsync()
 {
  var users = _userManager.Users.ToList();

  var list = new List<AdminUserDto>();

  foreach (var user in users)
  {
   var roles = await _userManager.GetRolesAsync(user);
   var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

   list.Add(new AdminUserDto
   {
    Id = user.Id,
    Email = user.Email,
    Username = user.UserName,
    FullName = user.FullName,
    Roles = roles.ToList(),
    IsLockedOut = isLocked
   });
  }

  return list;
 }

 public async Task<AdminUserDto> GetUserByIdAsync(int id)
 {
  var user = await _userManager.FindByIdAsync(id.ToString());
  if (user == null)
  {
   return null!;
  }

  var roles = await _userManager.GetRolesAsync(user);
  var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

  return new AdminUserDto
  {
   Id = user.Id,
   Email = user.Email,
   Username = user.UserName,
   FullName = user.FullName,
   Roles = roles.ToList(),
   IsLockedOut = isLocked
  };
 }

 public async Task<OperationResultDto> UpdateUserAsync(int id, AdminUpdateUserDto dto)
 {
  var user = await _userManager.FindByIdAsync(id.ToString());
  if (user == null)
  {
   return new OperationResultDto
   {
    Success = false,
    StatusCode = 404,
    Message = "User not found."
   };
  }

  var emailUser = await _userManager.FindByEmailAsync(dto.Email);
  if (emailUser != null && emailUser.Id != user.Id)
  {
   return new OperationResultDto
   {
    Success = false,
    StatusCode = 409,
    Message = "Email is already in use."
   };
  }

  var usernameUser = await _userManager.FindByNameAsync(dto.Username);
  if (usernameUser != null && usernameUser.Id != user.Id)
  {
   return new OperationResultDto
   {
    Success = false,
    StatusCode = 409,
    Message = "Username is already in use."
   };
  }

  user.FullName = dto.FullName;
  user.Email = dto.Email;
  user.UserName = dto.Username;

  if (dto.IsLockedOut)
  {
   user.LockoutEnd = DateTimeOffset.MaxValue;
  }
  else
  {
   user.LockoutEnd = null;
  }

  var updateResult = await _userManager.UpdateAsync(user);
  if (!updateResult.Succeeded)
  {
   return new OperationResultDto
   {
    Success = false,
    StatusCode = 400,
    Message = string.Join(". ", updateResult.Errors.Select(x => x.Description))
   };
  }

  // roles update
  var existingRoles = await _userManager.GetRolesAsync(user);
  var rolesToAdd = dto.Roles.Except(existingRoles).ToList();
  var rolesToRemove = existingRoles.Except(dto.Roles).ToList();

  // ensure roles exist
  foreach (var role in dto.Roles)
  {
   if (!await _roleManager.RoleExistsAsync(role))
   {
    return new OperationResultDto
    {
     Success = false,
     StatusCode = 400,
     Message = $"Role '{role}' does not exist."
    };
   }
  }

  if (rolesToRemove.Any())
  {
   await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
  }

  if (rolesToAdd.Any())
  {
   await _userManager.AddToRolesAsync(user, rolesToAdd);
  }

  return new OperationResultDto
  {
   Success = true,
   StatusCode = 200,
   Message = "User updated successfully."
  };
 }

 public async Task<OperationResultDto> DeleteUserAsync(int id)
 {
  var user = await _userManager.FindByIdAsync(id.ToString());
  if (user == null)
  {
   return new OperationResultDto
   {
    Success = false,
    StatusCode = 404,
    Message = "User not found."
   };
  }

  var result = await _userManager.DeleteAsync(user);
  if (!result.Succeeded)
  {
   return new OperationResultDto
   {
    Success = false,
    StatusCode = 400,
    Message = string.Join(". ", result.Errors.Select(x => x.Description))
   };
  }

  return new OperationResultDto
  {
   Success = true,
   StatusCode = 204,
   Message = "User deleted successfully."
  };
 }
}

