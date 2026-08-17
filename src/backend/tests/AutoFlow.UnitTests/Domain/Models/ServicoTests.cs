using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class ServicoTests
    {
        private const string NomeValido = "Troca de óleo";
        private const decimal PrecoValido = 150m;
        private const int TempoMedioValido = 60;

        [Fact]
        public void Construtor_ComDadosValidos_DevePreencherPropriedades()
        {
            var servico = new Servico(
                NomeValido,
                PrecoValido,
                TempoMedioValido);

            Assert.Equal(NomeValido, servico.Nome);
            Assert.Equal(PrecoValido, servico.Preco);
            Assert.Equal(TempoMedioValido, servico.TempoMedio);
        }

        [Fact]
        public void ConstrutorPadrao_DeveInicializarPropriedades()
        {
            var servico = new Servico();

            Assert.Null(servico.Nome);
            Assert.Equal(0, servico.Preco);
            Assert.Equal(0, servico.TempoMedio);
        }

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarPropriedades()
        {
            var servico = new Servico(
                NomeValido,
                PrecoValido,
                TempoMedioValido);

            const string novoNome = "Alinhamento";
            const decimal novoPreco = 200m;
            const int novoTempoMedio = 90;

            servico.Atualizar(
                novoNome,
                novoPreco,
                novoTempoMedio);

            Assert.Equal(novoNome, servico.Nome);
            Assert.Equal(novoPreco, servico.Preco);
            Assert.Equal(novoTempoMedio, servico.TempoMedio);
        }
    }
}