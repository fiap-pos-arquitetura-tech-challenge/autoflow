using AutoFlow.Domain.Validators;

namespace AutoFlow.UnitTests.Domain.Validators
{
    public class CpfValidadorTests
    {
        [Theory]
        [InlineData("11144477735")]
        [InlineData("111.444.777-35")]
        public void Validar_ComCpfValido_DeveRetornarTrue(string cpf)
        {
            Assert.True(CpfValidador.Validar(cpf));
        }

        [Theory]
        [InlineData("12345678901")]
        [InlineData("11144477736")]
        public void Validar_ComDigitosVerificadoresInvalidos_DeveRetornarFalse(string cpf)
        {
            Assert.False(CpfValidador.Validar(cpf));
        }

        [Theory]
        [InlineData("00000000000")]
        [InlineData("11111111111")]
        [InlineData("99999999999")]
        public void Validar_ComSequenciaRepetida_DeveRetornarFalse(string cpf)
        {
            Assert.False(CpfValidador.Validar(cpf));
        }

        [Theory]
        [InlineData("111444777")]
        [InlineData("111444777350")]
        public void Validar_ComTamanhoInvalido_DeveRetornarFalse(string cpf)
        {
            Assert.False(CpfValidador.Validar(cpf));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validar_ComCpfNuloOuVazio_DeveLancarArgumentException(string? cpf)
        {
            Assert.Throws<ArgumentException>(() => CpfValidador.Validar(cpf!));
        }
    }
}
