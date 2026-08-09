using AutoFlow.Api.Extensions;
using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Services;

namespace AutoFlow.Api.Endpoints
{
    public static class ServicoEndpoints
    {
        public static IEndpointRouteBuilder MapServicoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/servicos")
                .WithTags("Servicos");

            group.MapPost("/", Adicionar)
                .Produces<ServicoDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapPut("/{id:int}", Atualizar)
                .Produces<ServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);
            group.MapDelete("/{id:int}", Excluir)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            group.MapGet("/{id:int}", ObterPorId)
                .Produces<ServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            group.MapGet("/", ObterTodos)
                .Produces<IEnumerable<ServicoDto>>(StatusCodes.Status200OK);
            return app;
        }

        private static async Task<IResult> Excluir(
            int id,
            IServicoService servicoService)
        {
            var result = await servicoService.ExcluirAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> Adicionar(
            CriaServicoDto servicoDto,
            IServicoService servicoService)
        {
            var result = await servicoService.AdicionarAsync(servicoDto);

            return result.ToCreatedHttpResult(s => $"/servicos/{s.Id}");
        }

        private static async Task<IResult> Atualizar(
            int id,
            AtualizaServicoDto servicoDto,
            IServicoService servicoService)
        {
            var result = await servicoService.AtualizarAsync(id, servicoDto);

            return result.ToHttpResult();
        }

        private static async Task<IResult> ObterPorId(
            int id,
            IServicoService servicoService)
        {
            var result = await servicoService.ObterPorIdAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ObterTodos(
            IServicoService servicoService)
        {
            var servicos = await servicoService.ObterTodosAsync();
            return Results.Ok(servicos);
        }
    }
}
