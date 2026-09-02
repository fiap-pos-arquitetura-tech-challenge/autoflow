using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class ClienteRepositorio(AppDbContext db) : Repositorio<Cliente>(db), IClienteRepositorio
    {
        public async Task<Cliente?> ObterPorDocumentoAsync(string documento)
        {
            return await DbSet.FirstOrDefaultAsync(c => c.Documento.Numero == documento);
        }
    }
}
