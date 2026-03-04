using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using EchoWarehouse.Models.DTOs.Common;
using EchoWarehouse.Validators;

namespace EchoWarehouse.Extensions.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new List<ErrorDetail>();
                foreach (var kvp in context.ModelState)
                {
                    var state = kvp.Value;
                    if (state?.Errors != null && state.Errors.Count > 0)
                    {
                        var key = char.ToLower(kvp.Key[0]) + kvp.Key.Substring(1);
                        errors.Add(new ErrorDetail(key, state.Errors.First().ErrorMessage));
                    }
                }

                context.Result = new BadRequestObjectResult(new ApiErrorDto(
                    message: ValidationErrorKeys.ValidationFailed,
                    status: 400,
                    details: errors
                ));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}