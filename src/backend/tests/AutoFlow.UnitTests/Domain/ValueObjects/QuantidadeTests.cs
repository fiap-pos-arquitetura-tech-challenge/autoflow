using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects.Comum;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class QuantidadeTests
    {
        [Fact]
        public void Construtor_ComValorValido_DevePreencherValor()
        {
            var quantidade = new Quantidade(10);

            Assert.Equal(
                10,
                quantidade.Valor);
        }

        [Fact]
        public void Construtor_ComZero_DeveAceitarValor()
        {
            var quantidade = new Quantidade(0);

            Assert.Equal(
                0,
                quantidade.Valor);
        }

        [Fact]
        public void Construtor_ComValorNegativo_DeveLancarExcecao()
        {
            var exception =
                Assert.Throws<EstoqueInvalidoException>(
                    () => new Quantidade(-1));

            Assert.Equal(
                "Quantidade não pode ser negativa.",
                exception.Message);
        }
    }
}