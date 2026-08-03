using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class DocumentoTests
    {
        [Fact]
        public void Construtor_ComCpfValido_DeveCriarDocumentoComoCpf()
        {
            var documento = new Documento("12345678901");

            Assert.Equal("12345678901", documento.Numero);
            Assert.True(documento.EhCpf);
            Assert.False(documento.EhCnpj);
        }

        [Fact]
        public void Construtor_ComCnpjValido_DeveCriarDocumentoComoCnpj()
        {
            var documento = new Documento("12345678000199");

            Assert.Equal("12345678000199", documento.Numero);
            Assert.False(documento.EhCpf);
            Assert.True(documento.EhCnpj);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComNumeroNuloOuVazio_DeveLancarDocumentoInvalidoException(string? numero)
        {
            var exception = Assert.Throws<DocumentoInvalidoException>(() => new Documento(numero!));

            Assert.Equal("Número do documento é obrigatório.", exception.Message);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456789012")]
        [InlineData("1234567890123456")]
        public void Construtor_ComTamanhoInvalido_DeveLancarDocumentoInvalidoException(string numero)
        {
            var exception = Assert.Throws<DocumentoInvalidoException>(() => new Documento(numero));

            Assert.Equal("CPF/CNPJ inválido.", exception.Message);
        }
    }
}
