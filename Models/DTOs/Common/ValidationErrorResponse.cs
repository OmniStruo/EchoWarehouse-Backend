namespace EchoWarehouse.Models.DTOs.Common;

/// <summary>
/// Validation error details
/// </summary>
public class ValidationErrorDetail
{
    /// <summary>
    /// Field name where the error occurred
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Error code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    public ValidationErrorDetail()
    {
    }

    public ValidationErrorDetail(string field, string code, string message)
    {
        Field = field;
        Code = code;
        Message = message;
    }
}

/// <summary>
/// Validation error response
/// </summary>
public class ValidationErrorResponse
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int Status { get; set; } = 400;

    /// <summary>
    /// General error message
    /// </summary>
    public string Message { get; set; } = "Validation failed";

    /// <summary>
    /// List of validation errors by field
    /// </summary>
    public List<ValidationErrorDetail> Errors { get; set; } = new();

    public ValidationErrorResponse()
    {
    }

    public ValidationErrorResponse(string message, List<ValidationErrorDetail> errors)
    {
        Message = message;
        Errors = errors;
    }

    public ValidationErrorResponse(List<ValidationErrorDetail> errors)
    {
        Errors = errors;
    }
}

