namespace EchoWarehouse.Models.DTOs.Common;

public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<ErrorDetail>? Details { get; set; }

    // Success result
    public static Result<T> SuccessResult(T data) => new Result<T> 
    { 
        Success = true, 
        Data = data 
    };

    // Error result
    public static Result<T> ErrorResult(string message, List<ErrorDetail>? details = null) => new Result<T> 
    { 
        Success = false, 
        Message = message, 
        Details = details 
    };
}