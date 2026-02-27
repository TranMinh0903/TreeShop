using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PRN232.LaptopShop.API.Middleware
{
    public class FluentValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    IsSuccess = false,
                    Message = "Request body is missing or invalid JSON format.",
                    Errors = new { body = "The request body is required and cannot be empty or has invalid data types." }
                });
                return;
            }
            // 1. Kiểm tra xem có tham số nào được đánh dấu là [FromBody] mà bị null không
            var descriptor = context.ActionDescriptor.Parameters;

            foreach (var parameter in descriptor)
            {
                // Nếu tham số có giá trị null (do JSON thiếu hoặc sai format hoàn toàn)
                if (parameter.BindingInfo?.BindingSource == BindingSource.Body)
                {
                    // Nếu là Body mà thiếu hoàn toàn hoặc parse ra null
                    if (!context.ActionArguments.ContainsKey(parameter.Name))
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            IsSuccess = false,
                            Message = "Request body is missing or invalid JSON format.",
                            Errors = new { body = "The request body is required and cannot be empty." }
                        });
                        return;
                    }

                    var argument = context.ActionArguments[parameter.Name];

                    // 2. Nếu argument có giá trị, tiến hành chạy FluentValidation như cũ
                    var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                    var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                    if (validator != null)
                    {
                        var result = await validator.ValidateAsync(new ValidationContext<object>(argument));
                        if (!result.IsValid)
                        {
                            var errors = result.Errors
                                .GroupBy(e => e.PropertyName)
                                .ToDictionary(
                                    g => JsonNamingPolicy.CamelCase.ConvertName(g.Key),
                                    g => string.Join(", ", g.Select(e => e.ErrorMessage))
                                );

                            context.Result = new BadRequestObjectResult(new
                            {
                                IsSuccess = false,
                                Message = "Validation failed",
                                Errors = errors
                            });
                            return;
                        }
                    }
                }
            }
            await next();
        }
    }
}
