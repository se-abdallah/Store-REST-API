using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Store.Application.DTOs.Account;
using Store.Application.Interfaces;
using Store.Infrastructure.Identity;
using Store.Infrastructure.Services;

namespace Store.Infrastructure.Implementation;

public class AuthService : IAuthService
{

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var emailUser = await _userManager.FindByEmailAsync(dto.Email);
        if (emailUser != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                StatusCode = 409,
                Message = "Email is already in use."
            };
        }

        var usernameUser = await _userManager.FindByNameAsync(dto.Username);
        if (usernameUser != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                StatusCode = 409,
                Message = "Username is already in use."
            };
        }

        var user = new AppUser
        {
            Email = dto.Email,
            UserName = dto.Username,
            FullName = dto.FullName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                StatusCode = 400,
                Message = string.Join(". ", result.Errors.Select(x => x.Description))
            };
        }

        if (await _roleManager.RoleExistsAsync(RoleNames.Visitor))
        {
            await _userManager.AddToRoleAsync(user, RoleNames.Visitor);
        }

        return new AuthResponseDto
        {
            Success = true,
            StatusCode = 201,
            Message = "Registration successful.",
            Token = await _tokenService.CreateTokenAsync(user),
            User = await CreateCurrentUserDtoAsync(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        bool isEmail = dto.Identifier.Contains("@");

        AppUser? user = isEmail
            ? await _userManager.FindByEmailAsync(dto.Identifier)
            : await _userManager.FindByNameAsync(dto.Identifier);

        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                StatusCode = 404,
                Message = isEmail
                    ? "No user found with this email."
                    : "No user found with this username."
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                StatusCode = 401,
                Message = "Incorrect password."
            };
        }

        return new AuthResponseDto
        {
            Success = true,
            StatusCode = 200,
            Message = "Login successful.",
            Token = await _tokenService.CreateTokenAsync(user),
            User = await CreateCurrentUserDtoAsync(user)
        };
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null)
            return null;

        if (!int.TryParse(idClaim.Value, out var userId))
        {
            return null;
        }
        var user = await _userManager.FindByIdAsync(idClaim.Value);

        if (user == null)
            return null;

        return await CreateCurrentUserDtoAsync(user);
    }

    private async Task<CurrentUserDto> CreateCurrentUserDtoAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FullName,
            Roles = roles.ToList()
        };
    }

    public async Task<OperationResultDto> UpdateProfileAsync(ClaimsPrincipal principal, UpdateProfileDto dto)
    {
        var current = await GetCurrentUserAsync(principal);
        if (current == null)
        {
            return new OperationResultDto
            {
                Success = false,
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var user = await _userManager.FindByIdAsync(current.Id.ToString());
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

        return new OperationResultDto
        {
            Success = true,
            StatusCode = 200,
            Message = "Profile updated successfully."
        };
    }

    public async Task<OperationResultDto> ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordDto dto)
    {
        var current = await GetCurrentUserAsync(principal);
        if (current == null)
        {
            return new OperationResultDto
            {
                Success = false,
                StatusCode = 401,
                Message = "User is not authenticated."
            };
        }

        var user = await _userManager.FindByIdAsync(current.Id.ToString());
        if (user == null)
        {
            return new OperationResultDto
            {
                Success = false,
                StatusCode = 404,
                Message = "User not found."
            };
        }

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();

            return new OperationResultDto
            {
                Success = false,
                StatusCode = 400,
                Message = "Failed to change password.",
                Errors = errors
            };
        }

        return new OperationResultDto
        {
            Success = true,
            StatusCode = 200,
            Message = "Password changed successfully."
        };
    }
}
