using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepositorio(AppDbContext db) : Repositorio<Usuario>(db), IUsuarioRepositorio
    {
        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await DbSet.FirstOrDefaultAsync(u => u.Email.Endereco == email);
        }
    }
}
