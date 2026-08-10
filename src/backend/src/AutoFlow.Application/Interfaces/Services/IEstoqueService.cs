using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Application.Interfaces.Services;

public interface IEstoqueService
{
    Task<Result<EstoqueDto>> ObterPorIdAsync(
        int id);

    Task<Result<EstoqueDto>> ObterPorPecaInsumoAsync(
        int pecaInsumoId);

    Task<Result<EstoqueDto>> EntradaAsync(
        int id,
        MovimentaEstoqueDto dto);

    Task<Result<EstoqueDto>> SaidaAsync(
        int id,
        MovimentaEstoqueDto dto);

    Task<Result<EstoqueDto>> AjustarAsync(
        int id,
        MovimentaEstoqueDto dto);
}