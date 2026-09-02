using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Domain.Models;
using Moq;

namespace AutoFlow.UnitTests.Application.Services
{
    public class EstoqueServiceTests
    {
        private readonly Mock<IEstoqueRepositorio> _estoqueRepositorioMock = new();
        private readonly EstoqueService _service;

        public EstoqueServiceTests()
        {
            _service = new EstoqueService(
                _estoqueRepositorioMock.Object);
        }

        private static PecaInsumo PecaInsumoValida(int id = 1)
        {
            var peca = new PecaInsumo(
                "Pastilha de freio",
                100m);

            peca.Id = id;

            return peca;
        }

        private static Estoque EstoqueValido(
            int id = 1,
            int quantidade = 10)
        {
            var estoque = new Estoque(
                PecaInsumoValida(),
                quantidade);

            estoque.Id = id;

            return estoque;
        }

        [Fact]
        public async Task ObterPorIdAsync_ComEstoqueExistente_DeveRetornarSucessoComDto()
        {
            var estoque = EstoqueValido();

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(estoque.Id))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.ObterPorIdAsync(estoque.Id);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(
                estoque.Id,
                resultado.Value!.Id);

            Assert.Equal(
                estoque.Quantidade.Valor,
                resultado.Value.Quantidade);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComEstoqueInexistente_DeveRetornarFailureNotFound()
        {
            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Estoque?)null);

            var resultado =
                await _service.ObterPorIdAsync(1);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Estoque não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);
        }

        [Fact]
        public async Task ObterPorPecaInsumoAsync_ComEstoqueExistente_DeveRetornarSucesso()
        {
            var estoque = EstoqueValido();

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorPecaInsumoAsync(1))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.ObterPorPecaInsumoAsync(1);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(
                estoque.Id,
                resultado.Value!.Id);

            Assert.Equal(
                estoque.Quantidade.Valor,
                resultado.Value.Quantidade);
        }

        [Fact]
        public async Task ObterPorPecaInsumoAsync_ComEstoqueInexistente_DeveRetornarFailureNotFound()
        {
            _estoqueRepositorioMock
                .Setup(r => r.ObterPorPecaInsumoAsync(
                    It.IsAny<int>()))
                .ReturnsAsync((Estoque?)null);

            var resultado =
                await _service.ObterPorPecaInsumoAsync(1);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Estoque da peça/insumo não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);
        }

        [Fact]
        public async Task EntradaAsync_ComDadosValidos_DeveAdicionarQuantidadeEAtualizar()
        {
            var estoque = EstoqueValido(
                quantidade: 10);

            var dto = new MovimentaEstoqueDto(3);

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(estoque.Id))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.EntradaAsync(
                    estoque.Id,
                    dto);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(
                13,
                resultado.Value!.Quantidade);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(estoque),
                Times.Once);
        }

        [Fact]
        public async Task EntradaAsync_ComEstoqueInexistente_DeveRetornarNotFoundSemAtualizar()
        {
            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync((Estoque?)null);

            var dto = new MovimentaEstoqueDto(3);

            var resultado =
                await _service.EntradaAsync(1, dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Estoque não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(
                    It.IsAny<Estoque>()),
                Times.Never);
        }

        [Fact]
        public async Task EntradaAsync_ComQuantidadeInvalida_DeveRetornarValidationSemAtualizar()
        {
            var estoque = EstoqueValido(
                quantidade: 10);

            var dto = new MovimentaEstoqueDto(0);

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(estoque.Id))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.EntradaAsync(
                    estoque.Id,
                    dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                ErrorType.Validation,
                resultado.ErrorType);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(
                    It.IsAny<Estoque>()),
                Times.Never);
        }

        [Fact]
        public async Task SaidaAsync_ComDadosValidos_DeveDiminuirQuantidadeEAtualizar()
        {
            var estoque = EstoqueValido(
                quantidade: 10);

            var dto = new MovimentaEstoqueDto(3);

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(estoque.Id))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.SaidaAsync(
                    estoque.Id,
                    dto);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(
                7,
                resultado.Value!.Quantidade);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(estoque),
                Times.Once);
        }

        [Fact]
        public async Task SaidaAsync_ComEstoqueInexistente_DeveRetornarNotFoundSemAtualizar()
        {
            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync((Estoque?)null);

            var dto = new MovimentaEstoqueDto(3);

            var resultado =
                await _service.SaidaAsync(1, dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Estoque não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(
                    It.IsAny<Estoque>()),
                Times.Never);
        }

        [Fact]
        public async Task SaidaAsync_ComQuantidadeMaiorQueEstoque_DeveRetornarValidationSemAtualizar()
        {
            var estoque = EstoqueValido(
                quantidade: 5);

            var dto = new MovimentaEstoqueDto(6);

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(estoque.Id))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.SaidaAsync(
                    estoque.Id,
                    dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                ErrorType.Validation,
                resultado.ErrorType);

            Assert.Contains(
                "Estoque insuficiente",
                resultado.Error!);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(
                    It.IsAny<Estoque>()),
                Times.Never);
        }

        [Fact]
        public async Task SaidaAsync_ComQuantidadeZero_DeveRetornarValidationSemAtualizar()
        {
            var estoque = EstoqueValido();

            var dto = new MovimentaEstoqueDto(0);

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(estoque.Id))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.SaidaAsync(
                    estoque.Id,
                    dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                ErrorType.Validation,
                resultado.ErrorType);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(
                    It.IsAny<Estoque>()),
                Times.Never);
        }

        [Fact]
        public async Task AjustarAsync_ComDadosValidos_DeveAlterarQuantidadeEAtualizar()
        {
            var estoque = EstoqueValido(
                quantidade: 10);

            var dto = new MovimentaEstoqueDto(25);

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(estoque.Id))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.AjustarAsync(
                    estoque.Id,
                    dto);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(
                25,
                resultado.Value!.Quantidade);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(estoque),
                Times.Once);
        }

        [Fact]
        public async Task AjustarAsync_ComQuantidadeNegativa_DeveRetornarValidationSemAtualizar()
        {
            var estoque = EstoqueValido();

            var dto = new MovimentaEstoqueDto(-1);

            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(estoque.Id))
                .ReturnsAsync(estoque);

            var resultado =
                await _service.AjustarAsync(
                    estoque.Id,
                    dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                ErrorType.Validation,
                resultado.ErrorType);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(
                    It.IsAny<Estoque>()),
                Times.Never);
        }

        [Fact]
        public async Task AjustarAsync_ComEstoqueInexistente_DeveRetornarNotFoundSemAtualizar()
        {
            _estoqueRepositorioMock
                .Setup(r => r.ObterPorIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync((Estoque?)null);

            var dto = new MovimentaEstoqueDto(10);

            var resultado =
                await _service.AjustarAsync(1, dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Estoque não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);

            _estoqueRepositorioMock.Verify(
                r => r.AtualizarAsync(
                    It.IsAny<Estoque>()),
                Times.Never);
        }
    }
}