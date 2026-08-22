using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Interfaces.Repositories
{
    public interface IOrdemServicoRepositorio : IRepositorio<OrdemServico>
    {
        Task<OrdemServico?> ObterCompletaPorIdAsync(int id);
        Task<List<OrdemServico>> ObterTodasCompletasAsync();
    }
}
