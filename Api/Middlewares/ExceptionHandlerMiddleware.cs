using System.Net;
using System.Text.Json;
using Domain.DomainShared;

namespace Api.Middlewares;

/// <summary>
/// Middleware global de manejo de excepciones.
/// Captura todas las excepciones no controladas del pipeline de ASP.NET Core
/// y retorna una respuesta JSON uniforme usando el formato <see cref="ResponseApi{T}"/>.
///
/// Comportamiento:
///   - <see cref="DomainException"/> → HTTP 400 Bad Request  (error de negocio / validación)
///   - <see cref="Exception"/> genérica → HTTP 500 Internal Server Error (error inesperado)
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (DomainException ex)
        {
            // Error de dominio o validación: se expone el mensaje al cliente
            _logger.LogWarning(ex, "Domain exception caught: {Message}", ex.Message);
            await HandleExceptionAsync(
                httpContext,
                statusCode: HttpStatusCode.BadRequest,
                message: ex.Message);
        }
        catch (Exception ex)
        {
            // Error inesperado: se registra internamente sin exponer detalles al cliente
            _logger.LogError(ex, "Unhandled exception caught: {Message}", ex.Message);
            await HandleExceptionAsync(
                httpContext,
                statusCode: HttpStatusCode.InternalServerError,
                message: "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext httpContext,
        HttpStatusCode statusCode,
        string message)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)statusCode;

        var response = new ResponseApi<object>
        {
            Status = false,
            Message = message,
            Data = null!
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await httpContext.Response.WriteAsync(json);
    }
}
