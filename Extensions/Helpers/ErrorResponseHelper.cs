using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using EchoWarehouse.Models.DTOs.Common;

namespace EchoWarehouse.Extensions.Helpers;

/// <summary>
/// Helper class for consistent API error responses
/// </summary>
public static class ErrorResponseHelper
{
    /// <summary>
    /// Create a BadRequest response with standard error format
    /// </summary>
    public static BadRequestObjectResult BadRequestError(
        string message,
        List<ErrorDetail>? details = null)
    {
        var error = new ApiErrorDto(message, 400, details);
        return new BadRequestObjectResult(error);
    }

    /// <summary>
    /// Create a validation error response with field-level error details
    /// </summary>
    public static BadRequestObjectResult ValidationError(
        List<ValidationErrorDetail> errors,
        string message = "Validation failed")
    {
        var response = new ValidationErrorResponse(message, errors);
        return new BadRequestObjectResult(response);
    }

    /// <summary>
    /// Create a validation error response from ModelState
    /// </summary>
    public static BadRequestObjectResult ValidationErrorFromModelState(ModelStateDictionary modelState)
    {
        var errors = new List<ValidationErrorDetail>();

        foreach (var kvp in modelState)
        {
            var fieldName = kvp.Key;
            var state = kvp.Value;

            if (state?.Errors != null)
            {
                foreach (var error in state.Errors)
                {
                    errors.Add(new ValidationErrorDetail(
                        field: fieldName,
                        code: error.ErrorMessage.Split(':')[0].Trim(),
                        message: error.ErrorMessage
                    ));
                }
            }
        }

        return ValidationError(errors);
    }

    /// <summary>
    /// Create an Unauthorized response with standard error format
    /// </summary>
    public static UnauthorizedObjectResult UnauthorizedError(
        string message,
        List<ErrorDetail>? details = null)
    {
        var error = new ApiErrorDto(message, 401, details);
        return new UnauthorizedObjectResult(error);
    }

    /// <summary>
    /// Create a NotFound response with standard error format
    /// </summary>
    public static NotFoundObjectResult NotFoundError(
        string message,
        List<ErrorDetail>? details = null)
    {
        var error = new ApiErrorDto(message, 404, details);
        return new NotFoundObjectResult(error);
    }

    /// <summary>
    /// Create an InternalServerError response with standard error format
    /// </summary>
    public static ObjectResult InternalServerError(
        string message,
        List<ErrorDetail>? details = null)
    {
        var error = new ApiErrorDto(message, 500, details);
        return new ObjectResult(error) { StatusCode = 500 };
    }

    /// <summary>
    /// Create a Conflict response with standard error format
    /// </summary>
    public static ConflictObjectResult ConflictError(
        string message,
        List<ErrorDetail>? details = null)
    {
        var error = new ApiErrorDto(message, 409, details);
        return new ConflictObjectResult(error);
    }

    /// <summary>
    /// Create a custom status code response with standard error format
    /// </summary>
    public static ObjectResult CustomError(
        int statusCode,
        string message,
        List<ErrorDetail>? details = null)
    {
        var error = new ApiErrorDto(message, statusCode, details);
        return new ObjectResult(error) { StatusCode = statusCode };
    }
}



