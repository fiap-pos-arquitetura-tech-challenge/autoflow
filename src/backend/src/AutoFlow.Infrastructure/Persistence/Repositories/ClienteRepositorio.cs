using AutoFlow.Application.Persistence.Interfaces.Repositories;
using AutoFlow.Domain.Models;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class ClienteRepositorio : Repositorio<Cliente>, IClienteRepositorio
    {
        public ClienteRepositorio(AppDbContext db) : base(db)
        {
        }
    }
}
