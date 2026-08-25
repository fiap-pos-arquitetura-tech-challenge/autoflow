using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.Models
{
    public class OrdemServico : BaseModel
    {
        public int ClienteId { get; private set; }
        public int VeiculoId { get; private set; }
        public StatusOrdemServico Status { get; private set; }
        public string? AvariasObservadas { get; private set; }
        public string? Diagnostico { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public DateTime? DiagnosticoIniciadoEm { get; private set; }
        public DateTime? OrcamentoGeradoEm { get; private set; }
        public DateTime? OrcamentoDecididoEm { get; private set; }
        public DateTime? ExecucaoIniciadaEm { get; private set; }
        public DateTime? FinalizadaEm { get; private set; }
        public DateTime? EntregueEm { get; private set; }
        public ICollection<OrdemServicoItemServico> Servicos { get; private set; } = new List<OrdemServicoItemServico>();
        public ICollection<OrdemServicoItemPeca> Pecas { get; private set; } = new List<OrdemServicoItemPeca>();
        public Orcamento? Orcamento { get; private set; }

        public OrdemServico()
        {
        }

        public OrdemServico(int clienteId, int veiculoId, string? avariasObservadas = null)
        {
            if (clienteId <= 0)
                throw new OrdemServicoInvalidaException("Cliente é obrigatório.");

            if (veiculoId <= 0)
                throw new OrdemServicoInvalidaException("Veículo é obrigatório.");

            ClienteId = clienteId;
            VeiculoId = veiculoId;
            AvariasObservadas = string.IsNullOrWhiteSpace(avariasObservadas) ? "Sem avarias observadas" : avariasObservadas.Trim();
            Status = StatusOrdemServico.Recebida;
            DataAbertura = DateTime.UtcNow;
        }

        public void RegistrarAvarias(string? avariasObservadas)
        {
            ValidarStatus(StatusOrdemServico.Recebida, StatusOrdemServico.EmDiagnostico);
            AvariasObservadas = string.IsNullOrWhiteSpace(avariasObservadas) ? "Sem avarias observadas" : avariasObservadas.Trim();
        }

        public void IniciarDiagnostico()
        {
            ValidarStatus(StatusOrdemServico.Recebida);

            Status = StatusOrdemServico.EmDiagnostico;
            DiagnosticoIniciadoEm = DateTime.UtcNow;
        }

        public void RegistrarDiagnostico(string diagnostico)
        {
            ValidarStatus(StatusOrdemServico.EmDiagnostico);

            if (string.IsNullOrWhiteSpace(diagnostico))
                throw new OrdemServicoInvalidaException("Diagnóstico é obrigatório.");

            Diagnostico = diagnostico;
        }

        public void AdicionarServico(int servicoId, string descricao, int quantidade, decimal valorUnitario, int tempoPrevisto)
        {
            ValidarStatus(StatusOrdemServico.EmDiagnostico);

            Servicos.Add(new OrdemServicoItemServico(servicoId, descricao, quantidade, valorUnitario, tempoPrevisto));
        }

        public void RemoverServico(int servicoId)
        {
            ValidarStatus(StatusOrdemServico.EmDiagnostico);

            var item = Servicos.FirstOrDefault(x => x.ServicoId == servicoId);

            if (item is null)
                throw new OrdemServicoInvalidaException("Serviço não encontrado na ordem de serviço.");

            Servicos.Remove(item);
        }

        public void AdicionarPeca(int pecaId, string descricao, int quantidade, decimal valorUnitario)
        {
            ValidarStatus(StatusOrdemServico.EmDiagnostico);

            Pecas.Add(new OrdemServicoItemPeca(pecaId, descricao, quantidade, valorUnitario));
        }

        public void RemoverPeca(int pecaId)
        {
            ValidarStatus(StatusOrdemServico.EmDiagnostico);

            var item = Pecas.FirstOrDefault(x => x.PecaId == pecaId);

            if (item is null)
                throw new OrdemServicoInvalidaException("Peça não encontrada na ordem de serviço.");

            Pecas.Remove(item);
        }

        public void GerarOrcamento()
        {
            ValidarStatus(StatusOrdemServico.EmDiagnostico);

            if (string.IsNullOrWhiteSpace(Diagnostico))
                throw new OrdemServicoInvalidaException("O diagnóstico deve ser registrado antes de gerar o orçamento.");

            if (Servicos.Count == 0 && Pecas.Count == 0)
                throw new OrdemServicoInvalidaException("A ordem de serviço deve possuir ao menos um serviço ou peça.");

            var valorServicos = Servicos.Sum(x => x.Subtotal);
            var valorPecas = Pecas.Sum(x => x.Subtotal);

            Orcamento = new Orcamento(valorServicos, valorPecas);
            OrcamentoGeradoEm = Orcamento.GeradoEm;
            OrcamentoDecididoEm = null;
            Status = StatusOrdemServico.AguardandoAprovacao;
        }

        public void AprovarOrcamento()
        {
            ValidarStatus(StatusOrdemServico.AguardandoAprovacao);

            if (Orcamento is null)
                throw new OrdemServicoInvalidaException("Orçamento não encontrado.");

            Orcamento.Aprovar();
            OrcamentoDecididoEm = Orcamento.DecididoEm;
            ExecucaoIniciadaEm = DateTime.UtcNow;
            Status = StatusOrdemServico.EmExecucao;
        }

        public void IniciarExecucaoServico(int itemServicoId)
        {
            ValidarStatus(StatusOrdemServico.EmExecucao);

            var item = Servicos.FirstOrDefault(x => x.Id == itemServicoId);

            if (item is null)
                throw new OrdemServicoInvalidaException("Serviço não encontrado na ordem de serviço.");

            item.IniciarExecucao();
        }

        public void FinalizarExecucaoServico(int itemServicoId)
        {
            ValidarStatus(StatusOrdemServico.EmExecucao);

            var item = Servicos.FirstOrDefault(x => x.Id == itemServicoId);

            if (item is null)
                throw new OrdemServicoInvalidaException("Serviço não encontrado na ordem de serviço.");

            item.FinalizarExecucao();
        }

        public void ReprovarOrcamento(string justificativa)
        {
            ValidarStatus(StatusOrdemServico.AguardandoAprovacao);

            if (Orcamento is null)
                throw new OrdemServicoInvalidaException("Orçamento não encontrado.");

            Orcamento.Reprovar(justificativa);
            OrcamentoDecididoEm = Orcamento.DecididoEm;
            Status = StatusOrdemServico.EmDiagnostico;
        }

        public void Finalizar()
        {
            ValidarStatus(StatusOrdemServico.EmExecucao);

            if (Servicos.Any(x => x.ExecucaoFinalizadaEm is null))
                throw new OrdemServicoInvalidaException("Todos os serviços devem ser finalizados antes de finalizar a ordem de serviço.");

            Status = StatusOrdemServico.Finalizada;
            FinalizadaEm = DateTime.UtcNow;
        }

        public void Entregar()
        {
            ValidarStatus(StatusOrdemServico.Finalizada);

            Status = StatusOrdemServico.Entregue;
            EntregueEm = DateTime.UtcNow;
        }

        private void ValidarStatus(params StatusOrdemServico[] statusPermitidos)
        {
            if (!statusPermitidos.Contains(Status))
                throw new OrdemServicoInvalidaException($"A ação não é permitida para uma ordem de serviço com status {Status}.");
        }
    }
}
