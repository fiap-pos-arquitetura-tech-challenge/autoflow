using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class OrcamentoTests
    {
        [Fact]
        public void Construtor_ComValoresValidos_DeveCriarOrcamentoPendente()
        {
            var orcamento = new Orcamento(300m, 100m);

            Assert.Equal(StatusOrcamento.Pendente, orcamento.Status);
            Assert.Equal(300m, orcamento.ValorServicos);
            Assert.Equal(100m, orcamento.ValorPecas);
            Assert.Equal(400m, orcamento.ValorTotal);
            Assert.NotEqual(default, orcamento.GeradoEm);
            Assert.Null(orcamento.DecididoEm);
        }

        [Fact]
        public void Aprovar_OrcamentoPendente_DeveAtualizarStatus()
        {
            var orcamento = new Orcamento(300m, 100m);

            orcamento.Aprovar();

            Assert.Equal(StatusOrcamento.Aprovado, orcamento.Status);
            Assert.NotNull(orcamento.DecididoEm);
        }

        [Fact]
        public void Reprovar_SemJustificativa_DeveLancarOrdemServicoInvalidaException()
        {
            var orcamento = new Orcamento(300m, 100m);

            var exception = Assert.Throws<OrdemServicoInvalidaException>(() =>
                orcamento.Reprovar(""));

            Assert.Equal(
                "Justificativa da reprovação é obrigatória.",
                exception.Message);
        }
    }
}
