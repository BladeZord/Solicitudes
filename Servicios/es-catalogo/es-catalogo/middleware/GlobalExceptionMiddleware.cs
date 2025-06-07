using es_catalogo.Constans;
using es_catalogo.exception;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace es_catalogo.middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (ServiceException ex)
            {
                _logger.LogWarning(ex, "Error de negocio: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                Success = false,
                Message = exception.Message,
                ErrorCode = exception.GetType().Name
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
} 