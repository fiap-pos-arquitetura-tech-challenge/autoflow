using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Services;

public class EstoqueService(
    IEstoqueRepositorio estoqueRepositorio)
    : IEstoqueService
{
    private readonly IEstoqueRepositorio
        _estoqueRepositorio = estoqueRepositorio;

    public async Task<Result<EstoqueDto>> ObterPorIdAsync(
        int id)
    {
        var estoque =
            await _estoqueRepositorio.ObterPorIdAsync(id);

        if (estoque is null)
        {
            return Result<EstoqueDto>.Failure(
                "Estoque não encontrado.",
                ErrorType.NotFound);
        }

        return Result<EstoqueDto>.Success(
            MapearParaDto(estoque));
    }

    public async Task<Result<EstoqueDto>>
        ObterPorPecaInsumoAsync(int pecaInsumoId)
    {
        var estoque =
            await _estoqueRepositorio
                .ObterPorPecaInsumoAsync(pecaInsumoId);

        if (estoque is null)
        {
            return Result<EstoqueDto>.Failure(
                "Estoque da peça/insumo não encontrado.",
                ErrorType.NotFound);
        }

        return Result<EstoqueDto>.Success(
            MapearParaDto(estoque));
    }

    public async Task<Result<EstoqueDto>> EntradaAsync(
        int id,
        MovimentaEstoqueDto dto)
    {
        var estoque =
            await _estoqueRepositorio.ObterPorIdAsync(id);

        if (estoque is null)
        {
            return Result<EstoqueDto>.Failure(
                "Estoque não encontrado.",
                ErrorType.NotFound);
        }

        try
        {
            estoque.Entrada(dto.Quantidade);
        }
        catch (Exception ex)
        {
            return Result<EstoqueDto>.Failure(
                ex.Message,
                ErrorType.Validation);
        }

        await _estoqueRepositorio.AtualizarAsync(
            estoque);

        return Result<EstoqueDto>.Success(
            MapearParaDto(estoque));
    }

    public async Task<Result<EstoqueDto>> SaidaAsync(
        int id,
        MovimentaEstoqueDto dto)
    {
        var estoque =
            await _estoqueRepositorio.ObterPorIdAsync(id);

        if (estoque is null)
        {
            return Result<EstoqueDto>.Failure(
                "Estoque não encontrado.",
                ErrorType.NotFound);
        }

        try
        {
            estoque.Saida(dto.Quantidade);
        }
        catch (Exception ex)
        {
            return Result<EstoqueDto>.Failure(
                ex.Message,
                ErrorType.Validation);
        }

        await _estoqueRepositorio.AtualizarAsync(
            estoque);

        return Result<EstoqueDto>.Success(
            MapearParaDto(estoque));
    }

    public async Task<Result<EstoqueDto>> AjustarAsync(
        int id,
        MovimentaEstoqueDto dto)
    {
        var estoque =
            await _estoqueRepositorio.ObterPorIdAsync(id);

        if (estoque is null)
        {
            return Result<EstoqueDto>.Failure(
                "Estoque não encontrado.",
                ErrorType.NotFound);
        }

        try
        {
            estoque.Ajustar(dto.Quantidade);
        }
        catch (Exception ex)
        {
            return Result<EstoqueDto>.Failure(
                ex.Message,
                ErrorType.Validation);
        }

        await _estoqueRepositorio.AtualizarAsync(
            estoque);

        return Result<EstoqueDto>.Success(
            MapearParaDto(estoque));
    }

    private static EstoqueDto MapearParaDto(
        Estoque estoque)
    {
        return new EstoqueDto(
            estoque.Id,
            estoque.Quantidade.Valor);
    }
}