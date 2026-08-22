using AutoFlow.Domain.Enums;

namespace AutoFlow.Application.DTOs
{
    public record CriaOrdemServicoDto(int ClienteId, int VeiculoId, string? AvariasObservadas);

    public record RegistraAvariasOrdemServicoDto(string? AvariasObservadas);

    public record RegistraDiagnosticoOrdemServicoDto(string Diagnostico);

    public record AdicionaServicoOrdemServicoDto(int ServicoId, int Quantidade);

    public record AdicionaPecaOrdemServicoDto(int PecaId, int Quantidade);

    public record ReprovaOrcamentoOrdemServicoDto(string Justificativa);

    public record OrdemServicoItemServicoDto(int Id, int ServicoId, string Descricao, int Quantidade, decimal ValorUnitario, int TempoPrevisto, decimal Subtotal);

    public record OrdemServicoItemPecaDto(int Id, int PecaId, string Descricao, int Quantidade, decimal ValorUnitario, decimal Subtotal);

    public record OrcamentoDto(int Id, StatusOrcamento Status, decimal ValorServicos, decimal ValorPecas, decimal ValorTotal, DateTime GeradoEm, DateTime? DecididoEm, 
        string? JustificativaReprovacao);

    public record OrdemServicoDto(int Id, int ClienteId, int VeiculoId, StatusOrdemServico Status, string? AvariasObservadas, string? Diagnostico, DateTime DataAbertura,
        DateTime? DiagnosticoIniciadoEm, DateTime? OrcamentoGeradoEm, DateTime? OrcamentoDecididoEm, DateTime? ExecucaoIniciadaEm, DateTime? FinalizadaEm, DateTime? EntregueEm,
        IEnumerable<OrdemServicoItemServicoDto> Servicos, IEnumerable<OrdemServicoItemPecaDto> Pecas, OrcamentoDto? Orcamento);

    public record AndamentoOrdemServicoDto(int Id, StatusOrdemServico Status, DateTime DataAbertura, DateTime? DiagnosticoIniciadoEm, DateTime? OrcamentoGeradoEm,
        DateTime? OrcamentoDecididoEm, DateTime? ExecucaoIniciadaEm, DateTime? FinalizadaEm, DateTime? EntregueEm);
}
