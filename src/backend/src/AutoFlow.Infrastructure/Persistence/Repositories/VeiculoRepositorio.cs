using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class VeiculoRepositorio(AppDbContext db) : Repositorio<Veiculo>(db), IVeiculoRepositorio
    {
    }
}
