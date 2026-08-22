using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class OrdemServicoRepositorio(AppDbContext db)
        : Repositorio<OrdemServico>(db), IOrdemServicoRepositorio
    {
        public async Task<OrdemServico?> ObterCompletaPorIdAsync(int id)
        {
            return await DbSet
                .Include(x => x.Servicos)
                .Include(x => x.Pecas)
                .Include(x => x.Orcamento)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<OrdemServico>> ObterTodasCompletasAsync()
        {
            return await DbSet
                .Include(x => x.Servicos)
                .Include(x => x.Pecas)
                .Include(x => x.Orcamento)
                .ToListAsync();
        }
    }
}
