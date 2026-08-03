using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class ClienteRepositorio(AppDbContext db) : Repositorio<Cliente>(db), IClienteRepositorio
    {                
    }
}
