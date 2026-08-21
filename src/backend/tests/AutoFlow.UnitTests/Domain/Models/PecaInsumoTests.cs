using AutoFlow.Domain.Exceptions.PecaInsumo;
using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class PecaInsumoTests
    {
        [Fact]
        public void Construtor_ComDadosValidos_DevePreencherPropriedades()
        {
            var peca = new PecaInsumo(
                "Pastilha de freio",
                100m);

            Assert.Equal(
                "Pastilha de freio",
                peca.Nome);

            Assert.Equal(
                100m,
                peca.Valor.Valor);
        }

        [Fact]
        public void ConstrutorPadrao_DeveDeixarPropriedadesNulas()
        {
            var peca = new PecaInsumo();

            Assert.Null(peca.Nome);
            Assert.Null(peca.Valor);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComNomeInvalido_DeveLancarExcecao(
            string? nome)
        {
            var exception =
                Assert.Throws<PecaInsumoInvalidaException>(
                    () => new PecaInsumo(
                        nome!,
                        100m));

            Assert.Equal(
                "Nome é obrigatório.",
                exception.Message);
        }

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarPropriedades()
        {
            var peca = new PecaInsumo(
                "Pastilha de freio",
                100m);

            peca.Atualizar(
                "Disco de freio",
                250m);

            Assert.Equal(
                "Disco de freio",
                peca.Nome);

            Assert.Equal(
                250m,
                peca.Valor.Valor);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Atualizar_ComNomeInvalido_DeveLancarExcecaoSemAlterarEstado(
            string? nome)
        {
            var peca = new PecaInsumo(
                "Pastilha de freio",
                100m);

            Assert.Throws<PecaInsumoInvalidaException>(
                () => peca.Atualizar(
                    nome!,
                    250m));

            Assert.Equal(
                "Pastilha de freio",
                peca.Nome);

            Assert.Equal(
                100m,
                peca.Valor.Valor);
        }
    }
}