using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;

namespace AutoFlow.Application.Interfaces.Services
{
    public interface IVeiculoService
    {
        public Task<Result<VeiculoDto>> AdicionarAsync(CriaVeiculoDto veiculoDto);
        public Task<Result<VeiculoDto>> AtualizarAsync(int id, AtualizaVeiculoDto veiculoDto);
        public Task<Result> ExcluirAsync(int id);
        public Task<Result<VeiculoDto>> ObterPorIdAsync(int id);
        public Task<IEnumerable<VeiculoDto>> ObterTodosAsync();
    }
}
