using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class ServicoRepositorio : Repositorio<Servico>, IServicoRepositorio
    {
        public ServicoRepositorio(AppDbContext db) : base(db) { }

        public async Task<bool> ExistePorNomeAsync(string nome, int? id = null)         
        {
            return await Db.Set<Servico>()
                .AnyAsync(s =>
                    s.Nome == nome &&
                    (!id.HasValue || s.Id != id.Value));
        }
        public async Task<Servico?> ObterPorNomeAsync(string nome)
        {
            return await Db.Set<Servico>()
                .FirstOrDefaultAsync(s => s.Nome == nome);
        }
    }
}
