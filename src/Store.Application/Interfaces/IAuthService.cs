using System.Security.Claims;
using System.Threading.Tasks;
using Store.Application.DTOs.Account;

namespace Store.Application.Interfaces;

public interface IAuthService
{
 Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
 Task<AuthResponseDto> LoginAsync(LoginDto dto);
 Task<CurrentUserDto> GetCurrentUserAsync(ClaimsPrincipal user);
 Task<OperationResultDto> UpdateProfileAsync(ClaimsPrincipal user, UpdateProfileDto dto);
 Task<OperationResultDto> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordDto dto);
}
