using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class TelefoneTests
    {
        [Fact]
        public void Construtor_ComNumeroValido_DeveCriarTelefone()
        {
            var telefone = new Telefone("11999999999");

            Assert.Equal("11999999999", telefone.Numero);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComNumeroNuloOuVazio_DeveLancarTelefoneInvalidoException(string? numero)
        {
            var exception = Assert.Throws<TelefoneInvalidoException>(() => new Telefone(numero!));

            Assert.Equal("Telefone é obrigatório.", exception.Message);
        }

        [Theory]
        [InlineData("(11) 99999-9999", "11999999999")]
        [InlineData("(11) 9999-9999", "1199999999")]
        [InlineData("11 99999-9999", "11999999999")]
        public void Construtor_ComNumeroComMascara_DeveRemoverMascaraECriarTelefone(string numero, string numeroEsperado)
        {
            var telefone = new Telefone(numero);

            Assert.Equal(numeroEsperado, telefone.Numero);
        }

        [Theory]
        [InlineData("123456789")]
        [InlineData("123456789012")]
        public void Construtor_ComNumeroDeTamanhoInvalido_DeveLancarTelefoneInvalidoException(string numero)
        {
            var exception = Assert.Throws<TelefoneInvalidoException>(() => new Telefone(numero));

            Assert.Equal("Telefone precisa ter 10 ou 11 dígitos, incluindo o DDD.", exception.Message);
        }
    }
}
