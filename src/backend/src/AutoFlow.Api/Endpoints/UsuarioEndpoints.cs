using AutoFlow.Api.Extensions;
using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Domain.Enums;

namespace AutoFlow.Api.Endpoints
{
    public static class UsuarioEndpoints
    {
        public static IEndpointRouteBuilder MapUsuarioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usuarios")
                .WithTags("Usuarios");

            group.MapPost("/", CriarColaborador)
                .RequireAuthorization(policy => policy.RequireRole(nameof(Perfil.Colaborador)))
                .Produces<UsuarioDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/clientes", AtivarAcessoCliente)
                .Produces<UsuarioDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            return app;
        }

        private static async Task<IResult> CriarColaborador(
            CriaColaboradorDto usuarioDto,
            IUsuarioService usuarioService)
        {
            var result = await usuarioService.AdicionarColaboradorAsync(usuarioDto);

            return result.ToCreatedHttpResult(u => $"/api/usuarios/{u.Id}");
        }

        private static async Task<IResult> AtivarAcessoCliente(
            AtivaAcessoClienteDto ativaAcessoDto,
            IUsuarioService usuarioService)
        {
            var result = await usuarioService.AtivarAcessoClienteAsync(ativaAcessoDto);

            return result.ToCreatedHttpResult(u => $"/api/usuarios/{u.Id}");
        }
    }
}
