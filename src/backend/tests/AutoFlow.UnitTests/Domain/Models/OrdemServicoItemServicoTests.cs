using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class OrdemServicoItemServicoTests
    {
        [Fact]
        public void Construtor_ComDadosValidos_DevePreencherPropriedades()
        {
            var item = new OrdemServicoItemServico(
                servicoId: 1,
                descricao: "Troca de óleo",
                quantidade: 2,
                valorUnitario: 150m,
                tempoPrevisto: 60);

            Assert.Equal(1, item.ServicoId);
            Assert.Equal("Troca de óleo", item.Descricao);
            Assert.Equal(2, item.Quantidade);
            Assert.Equal(150m, item.ValorUnitario);
            Assert.Equal(60, item.TempoPrevisto);
            Assert.Equal(300m, item.Subtotal);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Construtor_ComQuantidadeInvalida_DeveLancarOrdemServicoInvalidaException(
            int quantidade)
        {
            var exception = Assert.Throws<OrdemServicoInvalidaException>(() =>
                new OrdemServicoItemServico(
                    1,
                    "Troca de óleo",
                    quantidade,
                    150m,
                    60));

            Assert.Equal(
                "Quantidade do serviço deve ser maior que zero.",
                exception.Message);
        }

        [Fact]
        public void IniciarEFinalizarExecucao_DeveRegistrarDatas()
        {
            var item = new OrdemServicoItemServico(
                1,
                "Troca de óleo",
                1,
                150m,
                60);

            item.IniciarExecucao();

            Assert.NotNull(item.ExecucaoIniciadaEm);
            Assert.Null(item.ExecucaoFinalizadaEm);

            item.FinalizarExecucao();

            Assert.NotNull(item.ExecucaoFinalizadaEm);
            Assert.True(item.ExecucaoFinalizadaEm!.Value >= item.ExecucaoIniciadaEm!.Value);
        }

        [Fact]
        public void FinalizarExecucao_SemIniciar_DeveLancarOrdemServicoInvalidaException()
        {
            var item = new OrdemServicoItemServico(
                1,
                "Troca de óleo",
                1,
                150m,
                60);

            var exception = Assert.Throws<OrdemServicoInvalidaException>(() =>
                item.FinalizarExecucao());

            Assert.Equal(
                "A execução do serviço deve ser iniciada antes de ser finalizada.",
                exception.Message);
        }
    }
}
