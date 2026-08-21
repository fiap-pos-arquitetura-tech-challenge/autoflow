using AutoFlow.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace AutoFlow.Api.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, detail) = exception switch
            {
                DomainException domainException => (domainException.StatusCode, domainException.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado.")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Erro não tratado ao processar a requisição.");

            await Results.Problem(detail: detail, statusCode: statusCode)
                .ExecuteAsync(httpContext);

            return true;
        }
    }
}
