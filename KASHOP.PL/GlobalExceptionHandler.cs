using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Diagnostics;

namespace KASHOP.PL
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var result = new Result<object>
            {
                Success = false,
                
                MessageProcessingHandler = exception.InnerException.Message
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
            return true;
        }
    }
}
