using System;

namespace Store.API.Errors;

public class ApiException
{
 public ApiException(int statusCode, string message, string details = null)
 {
  StatusCode = statusCode;
  Message = message;
  Details = details;
 }

 public int StatusCode { get; set; }
 public string Message { get; }
 public string Details { get; set; }
}
