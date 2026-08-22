using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;

namespace AutoFlow.Application.Interfaces.Services
{
    public interface IOrdemServicoService
    {
        Task<Result<OrdemServicoDto>> AdicionarAsync(CriaOrdemServicoDto dto);
        Task<Result<OrdemServicoDto>> RegistrarAvariasAsync(int id, RegistraAvariasOrdemServicoDto dto);
        Task<Result<OrdemServicoDto>> IniciarDiagnosticoAsync(int id);
        Task<Result<OrdemServicoDto>> RegistrarDiagnosticoAsync(int id, RegistraDiagnosticoOrdemServicoDto dto);
        Task<Result<OrdemServicoDto>> AdicionarServicoAsync(int id, AdicionaServicoOrdemServicoDto dto);
        Task<Result<OrdemServicoDto>> RemoverServicoAsync(int id, int servicoId);
        Task<Result<OrdemServicoDto>> AdicionarPecaAsync(int id, AdicionaPecaOrdemServicoDto dto);
        Task<Result<OrdemServicoDto>> RemoverPecaAsync(int id, int pecaId);
        Task<Result<OrdemServicoDto>> GerarOrcamentoAsync(int id);
        Task<Result<OrdemServicoDto>> AprovarOrcamentoAsync(int id);
        Task<Result<OrdemServicoDto>> ReprovarOrcamentoAsync(int id, ReprovaOrcamentoOrdemServicoDto dto);
        Task<Result<OrdemServicoDto>> FinalizarAsync(int id);
        Task<Result<OrdemServicoDto>> EntregarAsync(int id);
        Task<Result<OrdemServicoDto>> ObterPorIdAsync(int id);
        Task<IEnumerable<OrdemServicoDto>> ObterTodosAsync();
        Task<Result<AndamentoOrdemServicoDto>> ConsultarAndamentoAsync(int id);
    }
}
