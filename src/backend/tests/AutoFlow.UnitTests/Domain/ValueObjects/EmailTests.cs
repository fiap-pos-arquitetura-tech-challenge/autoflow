using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Construtor_ComEnderecoValido_DeveCriarEmail()
        {
            var email = new Email("joao@email.com");

            Assert.Equal("joao@email.com", email.Endereco);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComEnderecoNuloOuVazio_DeveLancarEmailInvalidoException(string? endereco)
        {
            var exception = Assert.Throws<EmailInvalidoException>(() => new Email(endereco!));

            Assert.Equal("Email é obrigatório.", exception.Message);
        }
    }
}
