using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Errors;
using Store.Application.DTOs;
using Store.Application.DTOs.Account;
using Store.Application.Interfaces;

namespace Store.API.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/admin/users")]
    public class AdminUsersController : BaseApiController
    {
        private readonly IUserAdminService _userAdminService;

        public AdminUsersController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetUsers()
        {
            var users = await _userAdminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdminUserDto>> GetUser(int id)
        {
            var user = await _userAdminService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found."));
            }

            return Ok(user);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<OperationResultDto>> UpdateUser(int id, AdminUpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                };

                return BadRequest(errorResponse);
            }

            var result = await _userAdminService.UpdateUserAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<OperationResultDto>> DeleteUser(int id)
        {
            var result = await _userAdminService.DeleteUserAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}