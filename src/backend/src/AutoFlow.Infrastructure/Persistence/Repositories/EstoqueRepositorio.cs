using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Infrastructure.Persistence.Repositories;

public class EstoqueRepositorio(AppDbContext db)
    : Repositorio<Estoque>(db), IEstoqueRepositorio
{
    public async Task<Estoque?> ObterPorPecaInsumoAsync(
        int pecaInsumoId)
    {
        return await DbSet
            .Include(e => e.PecaInsumo)
            .FirstOrDefaultAsync(e =>
                EF.Property<int>(
                    e,
                    "PecaInsumoId") == pecaInsumoId);
    }
}