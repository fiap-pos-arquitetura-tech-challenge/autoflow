using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.Models
{
    public class OrdemServicoItemPeca : BaseModel
    {
        public int PecaId { get; private set; }
        public string Descricao { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public decimal Subtotal => Quantidade * ValorUnitario;

        public OrdemServicoItemPeca()
        {
            Descricao = null!;
        }

        public OrdemServicoItemPeca(
            int pecaId,
            string descricao,
            int quantidade,
            decimal valorUnitario)
        {
            Validar(pecaId, descricao, quantidade, valorUnitario);

            PecaId = pecaId;
            Descricao = descricao;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        public void AtualizarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new OrdemServicoInvalidaException("Quantidade da peça deve ser maior que zero.");

            Quantidade = quantidade;
        }

        private static void Validar(
            int pecaId,
            string descricao,
            int quantidade,
            decimal valorUnitario)
        {
            if (pecaId <= 0)
                throw new OrdemServicoInvalidaException("Peça é obrigatória.");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new OrdemServicoInvalidaException("Descrição da peça é obrigatória.");

            if (quantidade <= 0)
                throw new OrdemServicoInvalidaException("Quantidade da peça deve ser maior que zero.");

            if (valorUnitario < 0)
                throw new OrdemServicoInvalidaException("Valor da peça não pode ser negativo.");
        }
    }
}
