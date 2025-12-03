using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Errors;
using Store.Application.DTOs.Account;
using Store.Application.Interfaces;

namespace Store.API.Controllers;

public class AccountController : BaseApiController
{
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
                _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
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

                var result = await _authService.RegisterAsync(dto);
                return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
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

                var result = await _authService.LoginAsync(dto);
                return StatusCode(result.StatusCode, result);
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<ActionResult<CurrentUserDto>> GetCurrentUser()
        {
                var currentUser = await _authService.GetCurrentUserAsync(User);
                if (currentUser == null)
                {
                        return Unauthorized(new ApiResponse(401, "User is not authenticated."));
                }

                return Ok(currentUser);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<OperationResultDto>> UpdateProfile(UpdateProfileDto dto)
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

                var result = await _authService.UpdateProfileAsync(User, dto);
                return StatusCode(result.StatusCode, result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<OperationResultDto>> ChangePassword(ChangePasswordDto dto)
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

                var result = await _authService.ChangePasswordAsync(User, dto);
                return StatusCode(result.StatusCode, result);
        }
}
