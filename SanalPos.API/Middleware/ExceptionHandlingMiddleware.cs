using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SanalPos.Application.Common.Exceptions;
using SanalPos.Domain.Exceptions;

namespace SanalPos.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly bool _isDevelopment;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _isDevelopment = env.IsDevelopment();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstek işlenirken bir hata oluştu: {Message}", ex.Message);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "İç hata detayı: {Message}", ex.InnerException.Message);
                }
                
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = GetStatusCode(exception);

            var response = new
            {
                title = GetTitle(exception),
                status = statusCode,
                detail = exception.Message,
                errors = GetErrors(exception),
                innerException = _isDevelopment ? exception.InnerException?.Message : null,
                stackTrace = _isDevelopment ? exception.StackTrace : null,
                sqlErrors = _isDevelopment ? GetSqlErrors(exception) : null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private static int GetStatusCode(Exception exception)
        {
            if (exception is ValidationException)
                return StatusCodes.Status400BadRequest;
            else if (exception is NotFoundException)
                return StatusCodes.Status404NotFound;
            else if (exception is ForbiddenAccessException)
                return StatusCodes.Status403Forbidden;
            else if (exception is DomainException)
                return StatusCodes.Status400BadRequest;
            else if (exception is DbUpdateException)
                return StatusCodes.Status500InternalServerError;
            else if (exception is DbUpdateConcurrencyException)
                return StatusCodes.Status409Conflict;
            else
                return StatusCodes.Status500InternalServerError;
        }

        private static string GetTitle(Exception exception)
        {
            if (exception is ValidationException)
                return "Doğrulama Hatası";
            else if (exception is NotFoundException)
                return "Kaynak Bulunamadı";
            else if (exception is ForbiddenAccessException)
                return "Erişim Reddedildi";
            else if (exception is DomainException)
                return "Domain Hatası";
            else if (exception is DbUpdateException)
                return "Veritabanı Güncelleme Hatası";
            else if (exception is DbUpdateConcurrencyException)
                return "Veritabanı Eşzamanlılık Hatası";
            else
                return "Sunucu Hatası";
        }

        private static object GetErrors(Exception exception)
        {
            return exception switch
            {
                ValidationException validationException => validationException.Errors,
                DbUpdateException dbUpdateException => new Dictionary<string, string[]>
                {
                    ["DbUpdateException"] = new string[] { dbUpdateException.Message }
                },
                _ => null
            };
        }
        
        private static object GetSqlErrors(Exception exception)
        {
            if (exception is DbUpdateException dbEx && dbEx.InnerException is SqlException sqlEx)
            {
                var errors = new Dictionary<string, string>();
                
                foreach (SqlError err in sqlEx.Errors)
                {
                    errors.Add($"SqlError_{err.Number}", $"{err.Message} (Line: {err.LineNumber})");
                }
                
                return errors;
            }
            
            return null;
        }
    }
} 