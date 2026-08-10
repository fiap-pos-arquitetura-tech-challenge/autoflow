using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Interfaces.Repositories;

public interface IEstoqueRepositorio
    : IRepositorio<Estoque>
{
    Task<Estoque?> ObterPorPecaInsumoAsync(
        int pecaInsumoId);
}