using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class DocumentoTests
    {
        [Fact]
        public void Construtor_ComCpfValido_DeveCriarDocumentoComoCpf()
        {
            var documento = new Documento("11144477735");

            Assert.Equal("11144477735", documento.Numero);
            Assert.True(documento.EhCpf);
            Assert.False(documento.EhCnpj);
        }

        [Fact]
        public void Construtor_ComCnpjValido_DeveCriarDocumentoComoCnpj()
        {
            var documento = new Documento("11222333000181");

            Assert.Equal("11222333000181", documento.Numero);
            Assert.False(documento.EhCpf);
            Assert.True(documento.EhCnpj);
        }

        [Fact]
        public void Construtor_ComCpfComMascara_DeveRemoverMascaraECriarDocumentoComoCpf()
        {
            var documento = new Documento("111.444.777-35");

            Assert.Equal("11144477735", documento.Numero);
            Assert.True(documento.EhCpf);
            Assert.False(documento.EhCnpj);
        }

        [Fact]
        public void Construtor_ComCnpjComMascara_DeveRemoverMascaraECriarDocumentoComoCnpj()
        {
            var documento = new Documento("11.222.333/0001-81");

            Assert.Equal("11222333000181", documento.Numero);
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

            Assert.Equal("Documento precisa ter entre 11 e 14 caracteres.", exception.Message);
        }

        [Theory]
        [InlineData("12345678901")]
        [InlineData("11111111111")]
        public void Construtor_ComCpfInvalido_DeveLancarDocumentoInvalidoExceptionComMensagemCpf(string numero)
        {
            var exception = Assert.Throws<DocumentoInvalidoException>(() => new Documento(numero));

            Assert.Equal("CPF inválido.", exception.Message);
        }

        [Theory]
        [InlineData("11222333000199")]
        [InlineData("11111111111111")]
        public void Construtor_ComCnpjInvalido_DeveLancarDocumentoInvalidoExceptionComMensagemCnpj(string numero)
        {
            var exception = Assert.Throws<DocumentoInvalidoException>(() => new Documento(numero));

            Assert.Equal("CNPJ inválido.", exception.Message);
        }
    }
}
