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
    }
}
