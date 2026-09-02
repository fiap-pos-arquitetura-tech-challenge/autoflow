using AutoFlow.Api.Extensions;
using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Domain.Enums;

namespace AutoFlow.Api.Endpoints;

public static class EstoqueEndpoints
{
    public static IEndpointRouteBuilder MapEstoqueEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/estoques")
            .RequireAuthorization(policy => policy.RequireRole(nameof(Perfil.Colaborador)))
            .WithTags("Estoques");

        group.MapGet("/{id:int}", ObterPorId)
            .Produces<EstoqueDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet(
                "/peca/{pecaInsumoId:int}",
                ObterPorPecaInsumo)
            .Produces<EstoqueDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost(
                "/{id:int}/entrada",
                Entrada)
            .Produces<EstoqueDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost(
                "/{id:int}/saida",
                Saida)
            .Produces<EstoqueDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut(
                "/{id:int}/ajustar",
                Ajustar)
            .Produces<EstoqueDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> ObterPorId(
        int id,
        IEstoqueService estoqueService)
    {
        var result =
            await estoqueService.ObterPorIdAsync(id);

        return result.ToHttpResult();
    }

    private static async Task<IResult> ObterPorPecaInsumo(
        int pecaInsumoId,
        IEstoqueService estoqueService)
    {
        var result =
            await estoqueService.ObterPorPecaInsumoAsync(
                pecaInsumoId);

        return result.ToHttpResult();
    }

    private static async Task<IResult> Entrada(
        int id,
        MovimentaEstoqueDto dto,
        IEstoqueService estoqueService)
    {
        var result =
            await estoqueService.EntradaAsync(
                id,
                dto);

        return result.ToHttpResult();
    }

    private static async Task<IResult> Saida(
        int id,
        MovimentaEstoqueDto dto,
        IEstoqueService estoqueService)
    {
        var result =
            await estoqueService.SaidaAsync(
                id,
                dto);

        return result.ToHttpResult();
    }

    private static async Task<IResult> Ajustar(
        int id,
        MovimentaEstoqueDto dto,
        IEstoqueService estoqueService)
    {
        var result =
            await estoqueService.AjustarAsync(
                id,
                dto);

        return result.ToHttpResult();
    }
}
