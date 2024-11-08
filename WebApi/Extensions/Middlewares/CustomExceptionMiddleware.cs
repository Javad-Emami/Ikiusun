using Domain.Common;
using System.Text.Json;

namespace WebApi.Extensions.Middlewares;

public class CustomExceptionMiddleware
{
    private readonly RequestDelegate next;
    public CustomExceptionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await this.next.Invoke(httpContext);
        }
        catch (CustomException ex)
        {
            await this.HandleExceptionAsync(httpContext,ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, CustomException exception)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception.Code;

        var apiResult = new CreateApiResult<int>(0,exception.Code, exception.Message);

        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        var jsonResult = JsonSerializer.Serialize(apiResult, serializeOptions);

        return context.Response.WriteAsync(jsonResult);
    }
}
