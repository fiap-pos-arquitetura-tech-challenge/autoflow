using AutoFlow.Api.Extensions;
using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Domain.Enums;

namespace AutoFlow.Api.Endpoints
{
    public static class VeiculoEndpoints
    {
        public static IEndpointRouteBuilder MapVeiculoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/veiculos")
                .RequireAuthorization(policy => policy.RequireRole(nameof(Perfil.Colaborador)))
                .WithTags("Veiculos");

            group.MapPost("/", Adicionar)
                .Produces<VeiculoDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapPut("/{id:int}", Atualizar)
                .Produces<VeiculoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);
            group.MapDelete("/{id:int}", Excluir)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            group.MapGet("/{id:int}", ObterPorId)
                .Produces<VeiculoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            group.MapGet("/", ObterTodos)
                .Produces<IEnumerable<VeiculoDto>>(StatusCodes.Status200OK);
            return app;
        }

        private static async Task<IResult> Excluir(
            int id,
            IVeiculoService veiculoService)
        {
            var result = await veiculoService.ExcluirAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> Adicionar(
            CriaVeiculoDto veiculoDto,
            IVeiculoService veiculoService)
        {
            var result = await veiculoService.AdicionarAsync(veiculoDto);

            return result.ToCreatedHttpResult(c => $"/veiculos/{c.Id}");
        }

        private static async Task<IResult> Atualizar(
            int id,
            AtualizaVeiculoDto veiculoDto,
            IVeiculoService veiculoService)
        {
            var result = await veiculoService.AtualizarAsync(id, veiculoDto);

            return result.ToHttpResult();
        }

        private static async Task<IResult> ObterPorId(
            int id,
            IVeiculoService veiculoService)
        {
            var result = await veiculoService.ObterPorIdAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ObterTodos(
            IVeiculoService veiculoService)
        {
            var veiculos = await veiculoService.ObterTodosAsync();
            return Results.Ok(veiculos);
        }
    }
}
