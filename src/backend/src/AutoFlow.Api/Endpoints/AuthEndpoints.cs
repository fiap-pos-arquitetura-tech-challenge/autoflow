using AutoFlow.Api.Extensions;
using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Services;

namespace AutoFlow.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth")
                .WithTags("Auth");

            group.MapPost("/login", Login)
                .Produces<LoginResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized);

            return app;
        }

        private static async Task<IResult> Login(
            LoginRequestDto loginDto,
            IUsuarioService usuarioService)
        {
            var result = await usuarioService.LoginAsync(loginDto);

            return result.ToHttpResult();
        }
    }
}
