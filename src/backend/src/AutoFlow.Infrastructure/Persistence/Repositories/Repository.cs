using AutoFlow.Domain.Models;
using AutoFlow.Infrastructure.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public abstract class Repositorio<TBaseModel> : IRepositorio<TBaseModel> where TBaseModel : BaseModel, new()
    {
        protected readonly AppDbContext Db;
        protected readonly DbSet<TBaseModel> DbSet;

        protected Repositorio(AppDbContext db)
        {
            Db = db;
            DbSet = db.Set<TBaseModel>();
        }

        public Task Adicionar(TBaseModel entidade)
        {
            DbSet.Add(entidade);
            return Db.SaveChangesAsync();
        }

        public Task Atualizar(TBaseModel entidade)
        {
            DbSet.Update(entidade);
            return Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<TBaseModel?> ObterPorId(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<List<TBaseModel>> ObterTodos()
        {
            return await DbSet.ToListAsync();
        }

        public async Task Remover(int id)
        {
            DbSet.Remove(new TBaseModel { Id = id });
            await Db.SaveChangesAsync();
        }
    }
}
