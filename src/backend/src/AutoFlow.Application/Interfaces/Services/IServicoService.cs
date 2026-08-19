using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;

namespace AutoFlow.Application.Interfaces.Services
{
    public interface IServicoService
    {
        public Task<Result<ServicoDto>> AdicionarAsync(CriaServicoDto servicoDto);
        public Task<Result<ServicoDto>> AtualizarAsync(int id, AtualizaServicoDto servicoDto);
        public Task<Result> ExcluirAsync(int id);
        public Task<Result<ServicoDto>> ObterPorIdAsync(int id);
        public Task<IEnumerable<ServicoDto>> ObterTodosAsync();
        Task<Result<ServicoDto>> ObterPorNomeAsync(string nome);
    }
}
