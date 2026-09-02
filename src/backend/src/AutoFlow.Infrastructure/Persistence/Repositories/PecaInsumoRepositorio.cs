using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;

namespace AutoFlow.Infrastructure.Persistence.Repositories;

public class PecaInsumoRepositorio(AppDbContext db) : Repositorio<PecaInsumo>(db), IPecaInsumoRepositorio
{
}