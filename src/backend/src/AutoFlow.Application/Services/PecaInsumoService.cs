using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;
using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Services;

public class PecaInsumoService(
    IPecaInsumoRepositorio pecaInsumoRepositorio,
    IEstoqueRepositorio estoqueRepositorio)
    : IPecaInsumoService
{
    private readonly IPecaInsumoRepositorio _pecaInsumoRepositorio =
        pecaInsumoRepositorio;

    private readonly IEstoqueRepositorio _estoqueRepositorio =
        estoqueRepositorio;

    public async Task<Result<PecaInsumoDto>> AdicionarAsync(
        CriaPecaInsumoDto pecaInsumoDto)
    {
        var validador = PecaInsumoValidador.Validar(pecaInsumoDto);

        if (validador is not null)
        {
            return Result<PecaInsumoDto>.Failure(
                validador.Error!,
                validador.ErrorType!);
        }

        var pecaInsumo = new PecaInsumo(
            pecaInsumoDto.Nome,
            pecaInsumoDto.Valor);

        await _pecaInsumoRepositorio.AdicionarAsync(pecaInsumo);

        // Cria automaticamente o estoque da peça com quantidade 0.
        var estoque = new Estoque(
            pecaInsumo,
            0);

        await _estoqueRepositorio.AdicionarAsync(estoque);

        return Result<PecaInsumoDto>.Success(
            MapearParaDto(pecaInsumo));
    }

    public async Task<Result<PecaInsumoDto>> AtualizarAsync(
        int id,
        AtualizaPecaInsumoDto pecaInsumoDto)
    {
        var validador = PecaInsumoValidador.Validar(pecaInsumoDto);

        if (validador is not null)
        {
            return Result<PecaInsumoDto>.Failure(
                validador.Error!,
                validador.ErrorType!);
        }

        var pecaInsumo =
            await _pecaInsumoRepositorio.ObterPorIdAsync(id);

        if (pecaInsumo is null)
        {
            return Result<PecaInsumoDto>.Failure(
                "Peça de insumo não encontrada.",
                ErrorType.NotFound);
        }

        pecaInsumo.Atualizar(
            pecaInsumoDto.Nome,
            pecaInsumoDto.Valor);

        await _pecaInsumoRepositorio.AtualizarAsync(pecaInsumo);

        return Result<PecaInsumoDto>.Success(
            MapearParaDto(pecaInsumo));
    }

    public async Task<Result> ExcluirAsync(int id)
    {
        var pecaInsumo =
            await _pecaInsumoRepositorio.ObterPorIdAsync(id);

        if (pecaInsumo is null)
        {
            return Result.Failure(
                "Peça de insumo não encontrada.",
                ErrorType.NotFound);
        }

        await _pecaInsumoRepositorio.ExcluirAsync(pecaInsumo);

        return Result.Success();
    }

    public async Task<Result<PecaInsumoDto>> ObterPorIdAsync(int id)
    {
        var pecaInsumo =
            await _pecaInsumoRepositorio.ObterPorIdAsync(id);

        if (pecaInsumo is null)
        {
            return Result<PecaInsumoDto>.Failure(
                "Peça de insumo não encontrada.",
                ErrorType.NotFound);
        }

        return Result<PecaInsumoDto>.Success(
            MapearParaDto(pecaInsumo));
    }

    public async Task<IEnumerable<PecaInsumoDto>> ObterTodosAsync()
    {
        var pecaInsumos =
            await _pecaInsumoRepositorio.ObterTodosAsync();

        return pecaInsumos.Select(MapearParaDto);
    }

    private static PecaInsumoDto MapearParaDto(
        PecaInsumo pecaInsumo)
    {
        return new PecaInsumoDto(
            pecaInsumo.Id,
            pecaInsumo.Nome,
            pecaInsumo.Valor.Valor);
    }
}