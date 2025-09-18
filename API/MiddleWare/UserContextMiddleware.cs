namespace Api.Middleware;

public class UserContextMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return context.Response.WriteAsync("Missing X-User-Id header");
        }

        context.Items["UserId"] = userId;
        return next(context);
    }
}
