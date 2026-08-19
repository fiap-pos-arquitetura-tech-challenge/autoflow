using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Models;
using Moq;

namespace AutoFlow.UnitTests.Application.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock = new();
        private readonly Mock<IClienteRepositorio> _clienteRepositorioMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _service = new UsuarioService(_usuarioRepositorioMock.Object, _clienteRepositorioMock.Object, _tokenServiceMock.Object);
        }

        private static Cliente ClienteValido(int id = 1) =>
            new("João da Silva", "11144477735", "11999999999", "joao@email.com") { Id = id };

        [Fact]
        public async Task AdicionarColaboradorAsync_ComDadosValidos_DeveAdicionarERetornarSucesso()
        {
            var dto = new CriaColaboradorDto("Maria Souza", "maria@email.com", "SenhaForte@123");

            var resultado = await _service.AdicionarColaboradorAsync(dto);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(dto.Nome, resultado.Value!.Nome);
            Assert.Equal(dto.Email, resultado.Value.Email);
            Assert.Equal(Perfil.Colaborador, resultado.Value.Perfil);
            Assert.Null(resultado.Value.ClienteId);
            _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task AdicionarColaboradorAsync_ComDadosInvalidos_DeveRetornarFailureSemChamarRepositorio()
        {
            var dto = new CriaColaboradorDto("", "maria@email.com", "SenhaForte@123");

            var resultado = await _service.AdicionarColaboradorAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
            _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AdicionarColaboradorAsync_ComEmailJaCadastrado_DeveRetornarFailureConflictSemAdicionar()
        {
            var dto = new CriaColaboradorDto("Maria Souza", "maria@email.com", "SenhaForte@123");

            _usuarioRepositorioMock.Setup(r => r.ObterPorEmailAsync(dto.Email))
                .ReturnsAsync(new Usuario("Outro", dto.Email, "SenhaForte@123", Perfil.Colaborador));

            var resultado = await _service.AdicionarColaboradorAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Já existe um usuário cadastrado com este email.", resultado.Error);
            Assert.Equal(ErrorType.Conflict, resultado.ErrorType);
            _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AtivarAcessoClienteAsync_ComClienteExistente_DeveAdicionarERetornarSucesso()
        {
            var cliente = ClienteValido();
            var dto = new AtivaAcessoClienteDto(cliente.Documento.Numero, cliente.Email.Endereco, "SenhaForte@123");

            _clienteRepositorioMock.Setup(r => r.ObterPorDocumentoAsync(dto.Documento)).ReturnsAsync(cliente);

            var resultado = await _service.AtivarAcessoClienteAsync(dto);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(cliente.Email.Endereco, resultado.Value!.Email);
            Assert.Equal(Perfil.Cliente, resultado.Value.Perfil);
            Assert.Equal(cliente.Id, resultado.Value.ClienteId);
            _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task AtivarAcessoClienteAsync_ComDocumentoInexistente_DeveRetornarFailureNotFound()
        {
            var dto = new AtivaAcessoClienteDto("11144477735", "cliente@email.com", "SenhaForte@123");

            _clienteRepositorioMock.Setup(r => r.ObterPorDocumentoAsync(dto.Documento)).ReturnsAsync((Cliente?)null);

            var resultado = await _service.AtivarAcessoClienteAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Cliente não encontrado.", resultado.Error);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);
            _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AtivarAcessoClienteAsync_ComAcessoJaAtivado_DeveRetornarFailureConflictSemAdicionar()
        {
            var cliente = ClienteValido();
            var dto = new AtivaAcessoClienteDto(cliente.Documento.Numero, cliente.Email.Endereco, "SenhaForte@123");

            _clienteRepositorioMock.Setup(r => r.ObterPorDocumentoAsync(dto.Documento)).ReturnsAsync(cliente);
            _usuarioRepositorioMock.Setup(r => r.ObterPorEmailAsync(cliente.Email.Endereco))
                .ReturnsAsync(new Usuario(cliente.Nome, cliente.Email.Endereco, "SenhaForte@123", Perfil.Cliente, cliente.Id));

            var resultado = await _service.AtivarAcessoClienteAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Acesso já foi ativado para este cliente.", resultado.Error);
            Assert.Equal(ErrorType.Conflict, resultado.ErrorType);
            _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AtivarAcessoClienteAsync_ComEmailDiferenteDoCliente_DeveRetornarFailureNotFoundSemAdicionar()
        {
            var cliente = ClienteValido();
            var dto = new AtivaAcessoClienteDto(cliente.Documento.Numero, "outro@email.com", "SenhaForte@123");

            _clienteRepositorioMock.Setup(r => r.ObterPorDocumentoAsync(dto.Documento)).ReturnsAsync(cliente);

            var resultado = await _service.AtivarAcessoClienteAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Cliente não encontrado.", resultado.Error);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);
            _usuarioRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ComCredenciaisValidas_DeveRetornarSucessoComToken()
        {
            var usuario = new Usuario("João da Silva", "joao@email.com", "SenhaForte@123", Perfil.Colaborador);
            var dto = new LoginRequestDto("joao@email.com", "SenhaForte@123");
            var token = new TokenGerado("token-gerado", DateTime.UtcNow.AddHours(1));

            _usuarioRepositorioMock.Setup(r => r.ObterPorEmailAsync(dto.Email)).ReturnsAsync(usuario);
            _tokenServiceMock.Setup(t => t.GerarToken(usuario)).Returns(token);

            var resultado = await _service.LoginAsync(dto);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(token.Token, resultado.Value!.Token);
            Assert.Equal(Perfil.Colaborador, resultado.Value.Perfil);
        }

        [Fact]
        public async Task LoginAsync_ComEmailInexistente_DeveRetornarFailureUnauthorizedComMensagemGenerica()
        {
            var dto = new LoginRequestDto("desconhecido@email.com", "SenhaForte@123");

            _usuarioRepositorioMock.Setup(r => r.ObterPorEmailAsync(dto.Email)).ReturnsAsync((Usuario?)null);

            var resultado = await _service.LoginAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Credenciais inválidas.", resultado.Error);
            Assert.Equal(ErrorType.Unauthorized, resultado.ErrorType);
        }

        [Fact]
        public async Task LoginAsync_ComSenhaIncorreta_DeveRetornarFailureUnauthorizedComAMesmaMensagemGenerica()
        {
            var usuario = new Usuario("João da Silva", "joao@email.com", "SenhaForte@123", Perfil.Colaborador);
            var dto = new LoginRequestDto("joao@email.com", "SenhaErrada@123");

            _usuarioRepositorioMock.Setup(r => r.ObterPorEmailAsync(dto.Email)).ReturnsAsync(usuario);

            var resultado = await _service.LoginAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Credenciais inválidas.", resultado.Error);
            Assert.Equal(ErrorType.Unauthorized, resultado.ErrorType);
        }
    }
}
