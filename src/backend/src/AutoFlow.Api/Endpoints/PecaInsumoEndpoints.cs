using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Api.Extensions;
using AutoFlow.Application.DTOs;

namespace AutoFlow.Api.Endpoints;

public static class PecaInsumoEndpoints
{
    public static IEndpointRouteBuilder MapPecaInsumoEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pecasInsumos")
            .WithTags("Pecas/Insumos");

        group.MapPost("/", Adicionar)
            .Produces<PecaInsumoDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:int}", Atualizar)
            .Produces<PecaInsumoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", Excluir)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id:int}", ObterPorId)
            .Produces<PecaInsumoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", ObterTodos)
            .Produces<IEnumerable<PecaInsumoDto>>(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<IResult> Adicionar(
        CriaPecaInsumoDto pecaInsumoDto,
        IPecaInsumoService pecaInsumoService)
    {
        var result =
            await pecaInsumoService.AdicionarAsync(pecaInsumoDto);

        return result.ToCreatedHttpResult(
            p => $"/api/pecasInsumos/{p.Id}");
    }

    private static async Task<IResult> Atualizar(
        int id,
        AtualizaPecaInsumoDto pecaInsumoDto,
        IPecaInsumoService pecaInsumoService)
    {
        var result =
            await pecaInsumoService.AtualizarAsync(
                id,
                pecaInsumoDto);

        return result.ToHttpResult();
    }

    private static async Task<IResult> Excluir(
        int id,
        IPecaInsumoService pecaInsumoService)
    {
        var result =
            await pecaInsumoService.ExcluirAsync(id);

        return result.ToHttpResult();
    }

    private static async Task<IResult> ObterPorId(
        int id,
        IPecaInsumoService pecaInsumoService)
    {
        var result =
            await pecaInsumoService.ObterPorIdAsync(id);

        return result.ToHttpResult();
    }

    private static async Task<IResult> ObterTodos(
        IPecaInsumoService pecaInsumoService)
    {
        var pecas =
            await pecaInsumoService.ObterTodosAsync();

        return Results.Ok(pecas);
    }
}