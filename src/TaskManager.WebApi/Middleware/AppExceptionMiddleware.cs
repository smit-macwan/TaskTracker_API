using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;

namespace TaskManager.WebApi.Middleware;

public sealed class AppExceptionMiddleware(RequestDelegate next, ILogger<AppExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (NotFoundException ex)
        {
            await WriteProblem(ctx, StatusCodes.Status404NotFound, ex);
        }
        catch (ValidationException ex)
        {
            await WriteProblem(ctx, StatusCodes.Status400BadRequest, ex);
        }
        catch (AppException ex)
        {
            await WriteProblem(ctx, StatusCodes.Status400BadRequest, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            await WriteProblem(ctx, StatusCodes.Status500InternalServerError, ex);
        }
    }

    private static async Task WriteProblem(HttpContext ctx, int status, Exception ex)
    {
        ctx.Response.ContentType = MediaTypeNames.Application.ProblemJson;
        ctx.Response.StatusCode = status;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = ex is AppException ? ex.Message : "An unexpected error occurred."
        };

        if (ex is AppException appEx)
        {
            problem.Extensions["code"] = appEx.ErrorCode;
        }

        await ctx.Response.WriteAsJsonAsync(problem);
    }
}

