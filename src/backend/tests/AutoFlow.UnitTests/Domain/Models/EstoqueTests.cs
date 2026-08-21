using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class EstoqueTests
    {
        private static PecaInsumo CriarPecaInsumo()
        {
            return new PecaInsumo(
                "Pastilha de freio",
                100m);
        }

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarEstoque()
        {
            var peca = CriarPecaInsumo();

            var estoque = new Estoque(
                peca,
                10);

            Assert.Equal(
                peca,
                estoque.PecaInsumo);

            Assert.Equal(
                10,
                estoque.Quantidade.Valor);
        }

        [Fact]
        public void Construtor_SemQuantidade_DeveCriarEstoqueComQuantidadeZero()
        {
            var peca = CriarPecaInsumo();

            var estoque = new Estoque(peca);

            Assert.Equal(
                0,
                estoque.Quantidade.Valor);
        }

        [Fact]
        public void Construtor_ComPecaInsumoNula_DeveLancarExcecao()
        {
            var exception =
                Assert.Throws<EstoqueInvalidoException>(
                    () => new Estoque(null!, 10));

            Assert.Equal(
                "Peça/insumo é obrigatório.",
                exception.Message);
        }

        [Fact]
        public void Construtor_ComQuantidadeInicialNegativa_DeveLancarExcecao()
        {
            var exception =
                Assert.Throws<EstoqueInvalidoException>(
                    () => new Estoque(
                        CriarPecaInsumo(),
                        -1));

            Assert.Equal(
                "Quantidade inicial não pode ser negativa.",
                exception.Message);
        }

        [Fact]
        public void Entrada_ComQuantidadeValida_DeveAumentarEstoque()
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            estoque.Entrada(5);

            Assert.Equal(
                15,
                estoque.Quantidade.Valor);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Entrada_ComQuantidadeInvalida_DeveLancarExcecao(
            int quantidade)
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            var exception =
                Assert.Throws<EstoqueInvalidoException>(
                    () => estoque.Entrada(quantidade));

            Assert.Equal(
                "A quantidade deve ser maior que zero.",
                exception.Message);

            Assert.Equal(
                10,
                estoque.Quantidade.Valor);
        }

        [Fact]
        public void Saida_ComQuantidadeValida_DeveDiminuirEstoque()
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            estoque.Saida(3);

            Assert.Equal(
                7,
                estoque.Quantidade.Valor);
        }

        [Fact]
        public void Saida_ComQuantidadeIgualAoEstoque_DeveZerarEstoque()
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            estoque.Saida(10);

            Assert.Equal(
                0,
                estoque.Quantidade.Valor);
        }

        [Fact]
        public void Saida_ComQuantidadeMaiorQueEstoque_DeveLancarExcecao()
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            var exception =
                Assert.Throws<EstoqueInvalidoException>(
                    () => estoque.Saida(11));

            Assert.Contains(
                "Estoque insuficiente.",
                exception.Message);

            Assert.Equal(
                10,
                estoque.Quantidade.Valor);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Saida_ComQuantidadeInvalida_DeveLancarExcecao(
            int quantidade)
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            var exception =
                Assert.Throws<EstoqueInvalidoException>(
                    () => estoque.Saida(quantidade));

            Assert.Equal(
                "A quantidade deve ser maior que zero.",
                exception.Message);

            Assert.Equal(
                10,
                estoque.Quantidade.Valor);
        }

        [Fact]
        public void Ajustar_ComQuantidadeValida_DeveAlterarEstoque()
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            estoque.Ajustar(25);

            Assert.Equal(
                25,
                estoque.Quantidade.Valor);
        }

        [Fact]
        public void Ajustar_ComZero_DeveZerarEstoque()
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            estoque.Ajustar(0);

            Assert.Equal(
                0,
                estoque.Quantidade.Valor);
        }

        [Fact]
        public void Ajustar_ComQuantidadeNegativa_DeveLancarExcecao()
        {
            var estoque = new Estoque(
                CriarPecaInsumo(),
                10);

            var exception =
                Assert.Throws<EstoqueInvalidoException>(
                    () => estoque.Ajustar(-1));

            Assert.Equal(
                "Quantidade de ajuste não pode ser negativa.",
                exception.Message);

            Assert.Equal(
                10,
                estoque.Quantidade.Valor);
        }
    }
}