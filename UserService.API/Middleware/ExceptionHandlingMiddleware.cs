namespace UserService.API.Middleware;

using FluentValidation;
using Application.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            await WriteAsync(context, StatusCodes.Status400BadRequest, new { error = "Помилка валідації", errors });
        }
        catch (UnauthorizedException ex)
        {
            await WriteAsync(context, StatusCodes.Status401Unauthorized, new { error = ex.Message });
        }
        catch (ConflictException ex)
        {
            await WriteAsync(context, StatusCodes.Status409Conflict, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Деталі тільки в лог, клієнту не показуємо внутрішні повідомлення
            _logger.LogError(ex, "Unhandled exception");
            await WriteAsync(context, StatusCodes.Status500InternalServerError, new { error = "Внутрішня помилка сервера" });
        }
    }

    private static Task WriteAsync(HttpContext context, int statusCode, object body)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(body);
    }
}
