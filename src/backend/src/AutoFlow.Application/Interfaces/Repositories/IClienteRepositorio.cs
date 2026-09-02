using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Interfaces.Repositories
{
    public interface IClienteRepositorio : IRepositorio<Cliente>
    {
        Task<Cliente?> ObterPorDocumentoAsync(string documento);
    }
}
