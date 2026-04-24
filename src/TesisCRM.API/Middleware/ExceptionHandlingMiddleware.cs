using System.Text.Json;
using TesisCRM.API.Models.Common;

namespace TesisCRM.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var payload = ApiResponse<string>.Fail("Ocurrió un error interno.", new() { ex.Message });
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
