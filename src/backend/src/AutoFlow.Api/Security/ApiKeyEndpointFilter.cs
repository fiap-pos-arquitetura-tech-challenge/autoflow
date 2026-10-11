namespace AutoFlow.Api.Security
{
    public class ApiKeyEndpointFilter(IConfiguration configuration) : IEndpointFilter
    {
        private const string HeaderName = "X-Api-Key";

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var apiKeyEsperada = configuration["Integracoes:ApiKey"];
            var apiKeyRecebida = context.HttpContext.Request.Headers[HeaderName].ToString();

            if (string.IsNullOrEmpty(apiKeyEsperada) || !string.Equals(apiKeyRecebida, apiKeyEsperada, StringComparison.Ordinal))
                return Results.Unauthorized();

            return await next(context);
        }
    }
}
