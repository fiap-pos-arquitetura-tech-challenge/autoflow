using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class ClienteTests
    {
        private const string NomeValido = "João da Silva";
        private const string DocumentoValidoCpf = "12345678901";
        private const string TelefoneValido = "11999999999";
        private const string EmailValido = "joao@email.com";

        [Fact]
        public void Construtor_ComDadosValidos_DevePreencherPropriedades()
        {
            var cliente = new Cliente(NomeValido, DocumentoValidoCpf, TelefoneValido, EmailValido);

            Assert.Equal(NomeValido, cliente.Nome);
            Assert.Equal(DocumentoValidoCpf, cliente.Documento.Numero);
            Assert.Equal(TelefoneValido, cliente.Telefone);
            Assert.Equal(EmailValido, cliente.Email);
        }

        [Fact]
        public void ConstrutorPadrao_DeveDeixarPropriedadesNulas()
        {
            var cliente = new Cliente();

            Assert.Null(cliente.Nome);
            Assert.Null(cliente.Documento);
            Assert.Null(cliente.Telefone);
            Assert.Null(cliente.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComNomeInvalido_DeveLancarClienteInvalidoException(string? nomeInvalido)
        {
            var exception = Assert.Throws<ClienteInvalidoException>(
                () => new Cliente(nomeInvalido!, DocumentoValidoCpf, TelefoneValido, EmailValido));

            Assert.Equal("Nome é obrigatório.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComTelefoneInvalido_DeveLancarClienteInvalidoException(string? telefoneInvalido)
        {
            var exception = Assert.Throws<ClienteInvalidoException>(
                () => new Cliente(NomeValido, DocumentoValidoCpf, telefoneInvalido!, EmailValido));

            Assert.Equal("Telefone é obrigatório.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComEmailInvalido_DeveLancarClienteInvalidoException(string? emailInvalido)
        {
            var exception = Assert.Throws<ClienteInvalidoException>(
                () => new Cliente(NomeValido, DocumentoValidoCpf, TelefoneValido, emailInvalido!));

            Assert.Equal("Email é obrigatório.", exception.Message);
        }

        [Fact]
        public void Construtor_ComDocumentoInvalido_DeveLancarDocumentoInvalidoException()
        {
            Assert.Throws<DocumentoInvalidoException>(
                () => new Cliente(NomeValido, "123", TelefoneValido, EmailValido));
        }

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarPropriedades()
        {
            var cliente = new Cliente(NomeValido, DocumentoValidoCpf, TelefoneValido, EmailValido);

            const string novoNome = "Maria Souza";
            const string novoDocumento = "12345678000199";
            const string novoTelefone = "11888888888";
            const string novoEmail = "maria@email.com";

            cliente.Atualizar(novoNome, novoDocumento, novoTelefone, novoEmail);

            Assert.Equal(novoNome, cliente.Nome);
            Assert.Equal(novoDocumento, cliente.Documento.Numero);
            Assert.Equal(novoTelefone, cliente.Telefone);
            Assert.Equal(novoEmail, cliente.Email);
        }

        [Fact]
        public void Atualizar_ComNomeInvalido_DeveLancarClienteInvalidoExceptionSemAlterarEstado()
        {
            var cliente = new Cliente(NomeValido, DocumentoValidoCpf, TelefoneValido, EmailValido);

            Assert.Throws<ClienteInvalidoException>(
                () => cliente.Atualizar("", DocumentoValidoCpf, TelefoneValido, EmailValido));

            Assert.Equal(NomeValido, cliente.Nome);
        }

        [Fact]
        public void Atualizar_ComDocumentoInvalido_DeveLancarDocumentoInvalidoException()
        {
            var cliente = new Cliente(NomeValido, DocumentoValidoCpf, TelefoneValido, EmailValido);

            Assert.Throws<DocumentoInvalidoException>(
                () => cliente.Atualizar(NomeValido, "abc", TelefoneValido, EmailValido));
        }
    }
}
