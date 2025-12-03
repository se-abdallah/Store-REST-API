using System.Collections.Generic;
using System.Threading.Tasks;
using Store.Application.DTOs.Account;

namespace Store.Application.Interfaces;

public interface IUserAdminService
{
 Task<IReadOnlyList<AdminUserDto>> GetAllUsersAsync();

 Task<AdminUserDto> GetUserByIdAsync(int id);

 Task<OperationResultDto> UpdateUserAsync(int id, AdminUpdateUserDto dto);

 Task<OperationResultDto> DeleteUserAsync(int id);
}
