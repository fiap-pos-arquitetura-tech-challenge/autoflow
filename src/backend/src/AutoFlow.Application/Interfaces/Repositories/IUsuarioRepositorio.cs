using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Interfaces.Repositories
{
    public interface IUsuarioRepositorio : IRepositorio<Usuario>
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
    }
}
