using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;

namespace AutoFlow.Application.Interfaces.Services
{
    public interface IClienteService
    {
        public Task<Result<ClienteDto>> AdicionarAsync(CriaClienteDto clienteDto);
        public Task<Result<ClienteDto>> AtualizarAsync(int id, AtualizaClienteDto clienteDto);
        public Task<Result> ExcluirAsync(int id);
        public Task<Result<ClienteDto>> ObterPorIdAsync(int id);
        public Task<IEnumerable<ClienteDto>> ObterTodosAsync();
    }
}
