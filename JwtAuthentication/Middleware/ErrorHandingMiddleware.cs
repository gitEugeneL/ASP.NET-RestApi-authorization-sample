using JwtAuthentication.Exceptions;

namespace JwtAuthentication.Middleware;

public class ErrorHandingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (AlreadyExistException exception)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsync(exception.Message);
        }
        catch (NotFoundException exception)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(exception.Message);
        }
        catch (UnauthorizedException exception)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(exception.Message);
        }
        catch (Exception exceptions)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong");
        }
    }
}