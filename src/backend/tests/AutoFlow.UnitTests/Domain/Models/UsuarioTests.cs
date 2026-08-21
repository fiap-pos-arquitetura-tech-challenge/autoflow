using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class UsuarioTests
    {
        private const string NomeValido = "João da Silva";
        private const string EmailValido = "joao@email.com";
        private const string SenhaValida = "SenhaForte@123";

        [Fact]
        public void Construtor_ColaboradorComDadosValidos_DevePreencherPropriedades()
        {
            var usuario = new Usuario(NomeValido, EmailValido, SenhaValida, Perfil.Colaborador);

            Assert.Equal(NomeValido, usuario.Nome);
            Assert.Equal(EmailValido, usuario.Email.Endereco);
            Assert.Equal(Perfil.Colaborador, usuario.Perfil);
            Assert.Null(usuario.ClienteId);
            Assert.True(usuario.Senha.Verificar(SenhaValida));
        }

        [Fact]
        public void Construtor_ClienteComClienteIdValido_DevePreencherPropriedades()
        {
            var usuario = new Usuario(NomeValido, EmailValido, SenhaValida, Perfil.Cliente, clienteId: 1);

            Assert.Equal(Perfil.Cliente, usuario.Perfil);
            Assert.Equal(1, usuario.ClienteId);
        }

        [Fact]
        public void ConstrutorPadrao_DeveDeixarPropriedadesNulas()
        {
            var usuario = new Usuario();

            Assert.Null(usuario.Nome);
            Assert.Null(usuario.Email);
            Assert.Null(usuario.Senha);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComNomeInvalido_DeveLancarUsuarioInvalidoException(string? nomeInvalido)
        {
            var exception = Assert.Throws<UsuarioInvalidoException>(
                () => new Usuario(nomeInvalido!, EmailValido, SenhaValida, Perfil.Colaborador));

            Assert.Equal("Nome é obrigatório.", exception.Message);
        }

        [Fact]
        public void Construtor_PerfilClienteSemClienteId_DeveLancarUsuarioInvalidoException()
        {
            var exception = Assert.Throws<UsuarioInvalidoException>(
                () => new Usuario(NomeValido, EmailValido, SenhaValida, Perfil.Cliente));

            Assert.Equal("Usuário do perfil Cliente precisa estar vinculado a um Cliente.", exception.Message);
        }

        [Fact]
        public void Construtor_PerfilColaboradorComClienteId_DeveLancarUsuarioInvalidoException()
        {
            var exception = Assert.Throws<UsuarioInvalidoException>(
                () => new Usuario(NomeValido, EmailValido, SenhaValida, Perfil.Colaborador, clienteId: 1));

            Assert.Equal("Usuário do perfil Colaborador não deve estar vinculado a um Cliente.", exception.Message);
        }

        [Fact]
        public void Construtor_ComEmailInvalido_DeveLancarEmailInvalidoException()
        {
            Assert.Throws<EmailInvalidoException>(
                () => new Usuario(NomeValido, "email-invalido", SenhaValida, Perfil.Colaborador));
        }

        [Fact]
        public void Construtor_ComSenhaInvalida_DeveLancarSenhaInvalidaException()
        {
            Assert.Throws<SenhaInvalidaException>(
                () => new Usuario(NomeValido, EmailValido, "123", Perfil.Colaborador));
        }
    }
}
