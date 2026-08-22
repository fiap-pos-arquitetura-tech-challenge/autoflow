using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.Models
{
    public class OrdemServicoItemServico : BaseModel
    {
        public int OrdemServicoId { get; private set; }
        public int ServicoId { get; private set; }
        public string Descricao { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public int TempoPrevisto { get; private set; }
        public DateTime? ExecucaoIniciadaEm { get; private set; }
        public DateTime? ExecucaoFinalizadaEm { get; private set; }
        public decimal Subtotal => Quantidade * ValorUnitario;

        public OrdemServicoItemServico()
        {
            Descricao = null!;
        }

        public OrdemServicoItemServico(
            int servicoId,
            string descricao,
            int quantidade,
            decimal valorUnitario,
            int tempoPrevisto)
        {
            Validar(servicoId, descricao, quantidade, valorUnitario, tempoPrevisto);

            ServicoId = servicoId;
            Descricao = descricao;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            TempoPrevisto = tempoPrevisto;
        }

        public void AtualizarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new OrdemServicoInvalidaException("Quantidade do serviço deve ser maior que zero.");

            Quantidade = quantidade;
        }

        public void IniciarExecucao()
        {
            if (ExecucaoIniciadaEm.HasValue)
                throw new OrdemServicoInvalidaException("Execução do serviço já foi iniciada.");

            ExecucaoIniciadaEm = DateTime.UtcNow;
        }

        public void FinalizarExecucao()
        {
            if (!ExecucaoIniciadaEm.HasValue)
                throw new OrdemServicoInvalidaException("A execução do serviço deve ser iniciada antes de ser finalizada.");

            if (ExecucaoFinalizadaEm.HasValue)
                throw new OrdemServicoInvalidaException("Execução do serviço já foi finalizada.");

            ExecucaoFinalizadaEm = DateTime.UtcNow;
        }

        private static void Validar(
            int servicoId,
            string descricao,
            int quantidade,
            decimal valorUnitario,
            int tempoPrevisto)
        {
            if (servicoId <= 0)
                throw new OrdemServicoInvalidaException("Serviço é obrigatório.");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new OrdemServicoInvalidaException("Descrição do serviço é obrigatória.");

            if (quantidade <= 0)
                throw new OrdemServicoInvalidaException("Quantidade do serviço deve ser maior que zero.");

            if (valorUnitario < 0)
                throw new OrdemServicoInvalidaException("Valor do serviço não pode ser negativo.");

            if (tempoPrevisto <= 0)
                throw new OrdemServicoInvalidaException("Tempo previsto do serviço deve ser maior que zero.");
        }
    }
}
