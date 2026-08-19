using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.Models
{
    public class Orcamento : BaseModel
    {
        public StatusOrcamento Status { get; private set; }
        public decimal ValorServicos { get; private set; }
        public decimal ValorPecas { get; private set; }
        public decimal ValorTotal => ValorServicos + ValorPecas;
        public DateTime GeradoEm { get; private set; }
        public DateTime? DecididoEm { get; private set; }
        public string? JustificativaReprovacao { get; private set; }

        public Orcamento()
        {
        }

        public Orcamento(decimal valorServicos, decimal valorPecas)
        {
            if (valorServicos < 0 || valorPecas < 0)
                throw new OrdemServicoInvalidaException("Os valores do orçamento não podem ser negativos.");

            ValorServicos = valorServicos;
            ValorPecas = valorPecas;
            Status = StatusOrcamento.Pendente;
            GeradoEm = DateTime.UtcNow;
        }

        public void Aprovar()
        {
            ValidarPendente();

            Status = StatusOrcamento.Aprovado;
            DecididoEm = DateTime.UtcNow;
            JustificativaReprovacao = null;
        }

        public void Reprovar(string justificativa)
        {
            ValidarPendente();

            if (string.IsNullOrWhiteSpace(justificativa))
                throw new OrdemServicoInvalidaException("Justificativa da reprovação é obrigatória.");

            Status = StatusOrcamento.Reprovado;
            DecididoEm = DateTime.UtcNow;
            JustificativaReprovacao = justificativa;
        }

        private void ValidarPendente()
        {
            if (Status != StatusOrcamento.Pendente)
                throw new OrdemServicoInvalidaException("O orçamento já foi decidido.");
        }
    }
}
