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

        [Theory]
        [InlineData("joao@email.com")]
        [InlineData("joao.silva@email.com.br")]
        [InlineData("joao+trabalho@sub.dominio.com")]
        [InlineData("j@d.co")]
        public void Construtor_ComEnderecoValido_NaoDeveLancarExcecao(string endereco)
        {
            var email = new Email(endereco);

            Assert.Equal(endereco, email.Endereco);
        }

        [Theory]
        [InlineData("email-sem-arroba.com")]
        [InlineData("@email.com")]
        [InlineData("joao@")]
        [InlineData("joao@email")]
        [InlineData("joao@email..com")]
        [InlineData("joao email@email.com")]
        public void Construtor_ComEnderecoInvalido_DeveLancarEmailInvalidoException(string endereco)
        {
            var exception = Assert.Throws<EmailInvalidoException>(() => new Email(endereco));

            Assert.Equal("Email inválido.", exception.Message);
        }
    }
}
