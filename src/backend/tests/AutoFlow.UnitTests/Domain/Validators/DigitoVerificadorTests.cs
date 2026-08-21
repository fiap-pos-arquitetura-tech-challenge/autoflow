using AutoFlow.Domain.Validators;

namespace AutoFlow.UnitTests.Domain.Validators
{
    public class DigitoVerificadorTests
    {
        [Fact]
        public void CalculaDigito_ComNumeroVazio_DeveRetornarVazio()
        {
            var digitoVerificador = new DigitoVerificador("");

            Assert.Equal("", digitoVerificador.CalculaDigito());
        }

        [Fact]
        public void CalculaDigito_SemSubstituicao_DeveRetornarResultadoBrutoDoModulo()
        {
            var digitoVerificador = new DigitoVerificador("123456789")
                .ComMultiplicadoresDeAte(2, 11);

            Assert.Equal("10", digitoVerificador.CalculaDigito());
        }

        [Fact]
        public void CalculaDigito_ComSubstituicao_DeveSubstituirResultadoMapeado()
        {
            var digitoVerificador = new DigitoVerificador("123456789")
                .ComMultiplicadoresDeAte(2, 11)
                .Substituindo("0", 10, 11);

            Assert.Equal("0", digitoVerificador.CalculaDigito());
        }

        [Fact]
        public void CalculaDigito_ReplicaCalculoDosDoisDigitosDoCpf()
        {
            var digitoVerificador = new DigitoVerificador("111444777")
                .ComMultiplicadoresDeAte(2, 11)
                .Substituindo("0", 10, 11);

            var primeiroDigito = digitoVerificador.CalculaDigito();
            Assert.Equal("3", primeiroDigito);

            digitoVerificador.AdicionaDigito(primeiroDigito);
            var segundoDigito = digitoVerificador.CalculaDigito();
            Assert.Equal("5", segundoDigito);
        }

        [Fact]
        public void ComMultiplicadoresDeAte_DeveRetornarMesmaInstanciaParaEncadeamento()
        {
            var digitoVerificador = new DigitoVerificador("123");

            var resultado = digitoVerificador.ComMultiplicadoresDeAte(2, 5);

            Assert.Same(digitoVerificador, resultado);
        }

        [Fact]
        public void Substituindo_DeveRetornarMesmaInstanciaParaEncadeamento()
        {
            var digitoVerificador = new DigitoVerificador("123");

            var resultado = digitoVerificador.Substituindo("0", 10, 11);

            Assert.Same(digitoVerificador, resultado);
        }
    }
}
