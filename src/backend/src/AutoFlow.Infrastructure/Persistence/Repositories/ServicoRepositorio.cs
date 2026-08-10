using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class ServicoRepositorio(AppDbContext db) : Repositorio<Servico>(db), IServicoRepositorio
    {
    }
}
