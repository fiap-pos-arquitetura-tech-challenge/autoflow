using AutoFlow.Domain.Exceptions.PecaInsumo;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class DinheiroTests
    {
        [Fact]
        public void Construtor_ComValorValido_DevePreencherValor()
        {
            var dinheiro = new Dinheiro(100m);

            Assert.Equal(100m, dinheiro.Valor);
        }

        [Fact]
        public void Construtor_ComValorDecimal_DeveArredondarParaDuasCasas()
        {
            var dinheiro = new Dinheiro(100.456m);

            Assert.Equal(100.46m, dinheiro.Valor);
        }

        [Fact]
        public void Construtor_ComZero_DeveAceitarValor()
        {
            var dinheiro = new Dinheiro(0m);

            Assert.Equal(0m, dinheiro.Valor);
        }

        [Fact]
        public void Construtor_ComValorNegativo_DeveLancarExcecao()
        {
            var exception =
                Assert.Throws<PecaInsumoInvalidaException>(
                    () => new Dinheiro(-1m));

            Assert.Equal(
                "Valor não pode ser negativo.",
                exception.Message);
        }

        [Fact]
        public void ConversaoImplicita_DeDinheiroParaDecimal_DeveRetornarValor()
        {
            var dinheiro = new Dinheiro(150.50m);

            decimal valor = dinheiro;

            Assert.Equal(150.50m, valor);
        }

        [Fact]
        public void ConversaoImplicita_DeDecimalParaDinheiro_DeveCriarObjeto()
        {
            Dinheiro dinheiro = 150.50m;

            Assert.Equal(
                150.50m,
                dinheiro.Valor);
        }
    }
}