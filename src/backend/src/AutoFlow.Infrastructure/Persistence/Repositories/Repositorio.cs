using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public abstract class Repositorio<TBaseModel>(AppDbContext db) : IRepositorio<TBaseModel> where TBaseModel : BaseModel, new()
    {
        protected readonly AppDbContext Db = db;
        protected readonly DbSet<TBaseModel> DbSet = db.Set<TBaseModel>();
                
        public Task AdicionarAsync(TBaseModel entidade)
        {
            DbSet.Add(entidade);
            return Db.SaveChangesAsync();
        }

        public Task AtualizarAsync(TBaseModel entidade)
        {
            DbSet.Update(entidade);
            return Db.SaveChangesAsync();
        }

        public async Task ExcluirAsync(TBaseModel entidade)
        {
            DbSet.Remove(entidade);
            await Db.SaveChangesAsync();
        }

        public async Task<TBaseModel?> ObterPorIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<List<TBaseModel>> ObterTodosAsync()
        {
            return await DbSet.ToListAsync();
        }
    }
}
