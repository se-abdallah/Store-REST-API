using System;

namespace Store.Application.DTOs.Account;

public class AuthResponseDto
{
 public bool Success { get; set; }
 public int StatusCode { get; set; }
 public string Message { get; set; }
 public string Token { get; set; }
 public CurrentUserDto User { get; set; }
}
