using AutoFlow.Domain.Validators;

namespace AutoFlow.UnitTests.Domain.Validators
{
    public class CnpjValidadorTests
    {
        [Theory]
        [InlineData("11222333000181")]
        [InlineData("11.222.333/0001-81")]
        public void Validar_ComCnpjNumericoValido_DeveRetornarTrue(string cnpj)
        {
            Assert.True(CnpjValidador.Validar(cnpj));
        }

        [Theory]
        [InlineData("AB123456000110")]
        [InlineData("ab123456000110")]
        public void Validar_ComCnpjAlfanumericoValido_DeveRetornarTrue(string cnpj)
        {
            Assert.True(CnpjValidador.Validar(cnpj));
        }

        [Fact]
        public void Validar_ComDigitosVerificadoresInvalidos_DeveRetornarFalse()
        {
            Assert.False(CnpjValidador.Validar("11222333000199"));
        }

        [Fact]
        public void Validar_ComCaractereNaoNumericoNosDigitosVerificadores_DeveRetornarFalse()
        {
            Assert.False(CnpjValidador.Validar("1122233300018A"));
        }

        [Fact]
        public void Validar_ComSequenciaRepetida_DeveRetornarFalse()
        {
            Assert.False(CnpjValidador.Validar("11111111111111"));
        }

        [Theory]
        [InlineData("1122233300018")]
        [InlineData("112223330001811")]
        public void Validar_ComTamanhoInvalido_DeveRetornarFalse(string cnpj)
        {
            Assert.False(CnpjValidador.Validar(cnpj));
        }

        [Fact]
        public void Validar_ComCnpjNulo_DeveRetornarFalse()
        {
            Assert.False(CnpjValidador.Validar(null!));
        }
    }
}
