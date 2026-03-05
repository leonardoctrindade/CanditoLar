using System.Net;
using CandidatoLar.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CandidatoLar.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail, extensions) = MapException(exception);

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (extensions is not null)
            foreach (var (key, value) in extensions)
                problem.Extensions[key] = value;

        if (context.Items.TryGetValue("CorrelationId", out var cid))
            problem.Extensions["correlationId"] = cid;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static (int Status, string Title, string Detail, Dictionary<string, object?>? Extensions)
        MapException(Exception exception) => exception switch
    {
        ValidationException ve => (
            (int)HttpStatusCode.BadRequest,
            "Validation Error",
            "One or more validation errors occurred.",
            new Dictionary<string, object?>
            {
                ["errors"] = ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => (object)g.Select(e => e.ErrorMessage).ToArray())
            }),

        EntityNotFoundException nfe => (
            (int)HttpStatusCode.NotFound,
            "Not Found",
            nfe.Message,
            null),

        DuplicateCpfException dce => (
            (int)HttpStatusCode.Conflict,
            "Conflict",
            dce.Message,
            null),

        DuplicatePhoneException dpe => (
            (int)HttpStatusCode.Conflict,
            "Conflict",
            dpe.Message,
            null),

        InvalidCpfException ice => (
            (int)HttpStatusCode.BadRequest,
            "Invalid CPF",
            ice.Message,
            null),

        DomainException de => (
            (int)HttpStatusCode.BadRequest,
            "Domain Rule Violation",
            de.Message,
            null),

        _ => (
            (int)HttpStatusCode.InternalServerError,
            "Internal Server Error",
            "An unexpected error occurred. Please try again later.",
            null)
    };
}
