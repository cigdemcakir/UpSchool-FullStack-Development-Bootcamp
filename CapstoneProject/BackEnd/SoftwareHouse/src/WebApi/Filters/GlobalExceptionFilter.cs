using Application.Common.Models.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace WebApi.Filters;

public class GlobalExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        ApiErrorDto apiErrorDto = new ApiErrorDto();

        switch (context.Exception)
        {
            case ValidationException validationException:

                var propertyNames = validationException.Errors
                    .Select(x => x.PropertyName)
                    .Distinct();

                foreach (var propertyName in propertyNames)
                {
                    var propertyFailures = validationException.Errors
                        .Where(e => e.PropertyName == propertyName)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    apiErrorDto.Errors.Add(new ErrorDto(propertyName, propertyFailures));
                }

                apiErrorDto.Message = "Validation errors occured.";

                //context.HttpContext.Response.StatusCode=(int)StatusCodes.Status400BadRequest;
                context.Result = new BadRequestObjectResult(apiErrorDto);

                break;

            default:
                apiErrorDto.Message = "Unidentified error accured.";
                //context.HttpContext.Response.StatusCode=(int)StatusCodes.Status500InternalServerError;
                context.Result = new ObjectResult(apiErrorDto)
                {
                    StatusCode = (int)StatusCodes.Status500InternalServerError
                };
                break;
        }

        return Task.CompletedTask;
    }
}