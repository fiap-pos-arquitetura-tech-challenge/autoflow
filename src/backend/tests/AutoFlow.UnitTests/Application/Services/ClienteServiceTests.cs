using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Domain.Models;
using Moq;

namespace AutoFlow.UnitTests.Application.Services
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepositorio> _clienteRepositorioMock = new();
        private readonly ClienteService _service;

        public ClienteServiceTests()
        {
            _service = new ClienteService(_clienteRepositorioMock.Object);
        }

        private static Cliente ClienteValido(int id = 1) =>
            new("João da Silva", "12345678901", "11999999999", "joao@email.com") { Id = id };

        [Fact]
        public async Task AdicionarAsync_ComDadosValidos_DeveAdicionarERetornarSucesso()
        {
            var dto = new CriaClienteDto("João da Silva", "12345678901", "11999999999", "joao@email.com");

            var resultado = await _service.AdicionarAsync(dto);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(dto.Nome, resultado.Value!.Nome);
            Assert.Equal(dto.Documento, resultado.Value.Documento);
            Assert.Equal(dto.Telefone, resultado.Value.Telefone);
            Assert.Equal(dto.Email, resultado.Value.Email);
            _clienteRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task AdicionarAsync_ComDadosInvalidos_DeveRetornarFailureSemChamarRepositorio()
        {
            var dto = new CriaClienteDto("", "12345678901", "11999999999", "joao@email.com");

            var resultado = await _service.AdicionarAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
            _clienteRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComClienteExistenteEDadosValidos_DeveAtualizarERetornarSucesso()
        {
            var cliente = ClienteValido();
            var dto = new AtualizaClienteDto("Maria Souza", "12345678000199", "11888888888", "maria@email.com");

            _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

            var resultado = await _service.AtualizarAsync(cliente.Id, dto);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(dto.Nome, resultado.Value!.Nome);
            Assert.Equal(dto.Documento, resultado.Value.Documento);
            _clienteRepositorioMock.Verify(r => r.AtualizarAsync(cliente), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComDadosInvalidos_DeveRetornarFailureSemBuscarCliente()
        {
            var dto = new AtualizaClienteDto("", "12345678000199", "11888888888", "maria@email.com");

            var resultado = await _service.AtualizarAsync(1, dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
            _clienteRepositorioMock.Verify(r => r.ObterPorIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComClienteInexistente_DeveRetornarFailureNotFound()
        {
            var dto = new AtualizaClienteDto("Maria Souza", "12345678000199", "11888888888", "maria@email.com");

            _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Cliente?)null);

            var resultado = await _service.AtualizarAsync(1, dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Cliente não encontrado.", resultado.Error);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);
            _clienteRepositorioMock.Verify(r => r.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task ExcluirAsync_ComClienteExistente_DeveExcluirERetornarSucesso()
        {
            var cliente = ClienteValido();

            _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

            var resultado = await _service.ExcluirAsync(cliente.Id);

            Assert.True(resultado.IsSuccess);
            _clienteRepositorioMock.Verify(r => r.ExcluirAsync(cliente), Times.Once);
        }

        [Fact]
        public async Task ExcluirAsync_ComClienteInexistente_DeveRetornarFailureNotFound()
        {
            _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Cliente?)null);

            var resultado = await _service.ExcluirAsync(1);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Cliente não encontrado.", resultado.Error);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);
            _clienteRepositorioMock.Verify(r => r.ExcluirAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComClienteExistente_DeveRetornarSucessoComDto()
        {
            var cliente = ClienteValido();

            _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

            var resultado = await _service.ObterPorIdAsync(cliente.Id);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(cliente.Id, resultado.Value!.Id);
            Assert.Equal(cliente.Nome, resultado.Value.Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComClienteInexistente_DeveRetornarFailureNotFound()
        {
            _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Cliente?)null);

            var resultado = await _service.ObterPorIdAsync(1);

            Assert.False(resultado.IsSuccess);
            Assert.Equal("Cliente não encontrado.", resultado.Error);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodosOsClientesComoDto()
        {
            var clientes = new List<Cliente> { ClienteValido(1), ClienteValido(2) };

            _clienteRepositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(clientes);

            var resultado = (await _service.ObterTodosAsync()).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Equal(clientes[0].Id, resultado[0].Id);
            Assert.Equal(clientes[1].Id, resultado[1].Id);
        }

        [Fact]
        public async Task ObterTodosAsync_SemClientes_DeveRetornarListaVazia()
        {
            _clienteRepositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync([]);

            var resultado = await _service.ObterTodosAsync();

            Assert.Empty(resultado);
        }
    }
}
