namespace EchoWarehouse.Models.DTOs.Common;

/// <summary>
/// Standard API error response structure
/// </summary>
public class ApiErrorDto
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Additional error details
    /// </summary>
    public List<ErrorDetail>? Details { get; set; }

    public ApiErrorDto()
    {
    }

    public ApiErrorDto(string message, int? status = null, List<ErrorDetail>? details = null)
    {
        Message = message;
        Status = status;
        Details = details;
    }
}


