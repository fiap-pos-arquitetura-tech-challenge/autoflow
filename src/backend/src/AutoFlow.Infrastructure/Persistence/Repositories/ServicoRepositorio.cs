using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class ServicoRepositorio(AppDbContext db) : Repositorio<Servico>(db), IServicoRepositorio
    {
        public async Task<bool> ExistePorNomeAsync(string nome, int? id = null)
        {
            return await db.Set<Servico>()
                .AnyAsync(s =>
                    s.Nome == nome &&
                    (!id.HasValue || s.Id != id.Value));
        }
        public async Task<Servico?> ObterPorNomeAsync(string nome)
        {
            return await db.Set<Servico>()
                .FirstOrDefaultAsync(s => s.Nome == nome);
        }
    }
}
