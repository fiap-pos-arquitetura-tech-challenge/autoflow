using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Interfaces.Repositories
{
    public interface IServicoRepositorio : IRepositorio<Servico>
    {
        Task<bool> ExistePorNomeAsync(string nome, int? id = null);
        Task<Servico?> ObterPorNomeAsync(string nome);
    }
}
