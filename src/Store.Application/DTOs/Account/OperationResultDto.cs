using System;

namespace Store.Application.DTOs.Account;

public class OperationResultDto
{
 public bool Success { get; set; }
 public int StatusCode { get; set; }
 public string Message { get; set; }
 public IEnumerable<string> Errors { get; set; } = new List<string>();
}
