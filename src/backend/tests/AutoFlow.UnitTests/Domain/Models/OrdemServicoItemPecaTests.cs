using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class OrdemServicoItemPecaTests
    {
        [Fact]
        public void Construtor_ComDadosValidos_DevePreencherPropriedades()
        {
            var item = new OrdemServicoItemPeca(
                pecaId: 1,
                descricao: "Filtro de óleo",
                quantidade: 2,
                valorUnitario: 50m);

            Assert.Equal(1, item.PecaId);
            Assert.Equal("Filtro de óleo", item.Descricao);
            Assert.Equal(2, item.Quantidade);
            Assert.Equal(50m, item.ValorUnitario);
            Assert.Equal(100m, item.Subtotal);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Construtor_ComQuantidadeInvalida_DeveLancarOrdemServicoInvalidaException(
            int quantidade)
        {
            var exception = Assert.Throws<OrdemServicoInvalidaException>(() =>
                new OrdemServicoItemPeca(
                    1,
                    "Filtro de óleo",
                    quantidade,
                    50m));

            Assert.Equal(
                "Quantidade da peça deve ser maior que zero.",
                exception.Message);
        }
    }
}
