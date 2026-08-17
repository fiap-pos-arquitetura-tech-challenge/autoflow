using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Domain.Models;
using Moq;

namespace AutoFlow.UnitTests.Application.Services
{
    public class ServicoServiceTests
    {
        private readonly Mock<IServicoRepositorio> _servicoRepositorioMock = new();
        private readonly ServicoService _service;

        public ServicoServiceTests()
        {
            _service = new ServicoService(
                _servicoRepositorioMock.Object);
        }

        private static Servico ServicoValido(int id = 1) =>
            new("Troca de óleo", 150m, 60)
            {
                Id = id
            };

        private static CriaServicoDto DtoValido() =>
            new("Troca de óleo", 150m, 60);

        // ============================================================
        // ADICIONAR
        // ============================================================

        [Fact]
        public async Task AdicionarAsync_ComDadosValidos_DeveAdicionarERetornarSucesso()
        {
            var dto = DtoValido();

            _servicoRepositorioMock
                .Setup(r => r.ExistePorNomeAsync(dto.Nome.Trim(), null))
                .ReturnsAsync(false);

            var resultado = await _service.AdicionarAsync(dto);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(dto.Nome, resultado.Value!.Nome);
            Assert.Equal(dto.Preco, resultado.Value.Preco);
            Assert.Equal(dto.TempoMedio, resultado.Value.TempoMedio);

            _servicoRepositorioMock.Verify(
                r => r.ExistePorNomeAsync(dto.Nome.Trim(), null),
                Times.Once);

            _servicoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Servico>()),
                Times.Once);
        }

        [Fact]
        public async Task AdicionarAsync_ComDadosInvalidos_DeveRetornarFailureSemChamarRepositorio()
        {
            var dto = DtoValido() with
            {
                Nome = ""
            };

            var resultado = await _service.AdicionarAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.ExistePorNomeAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()),
                Times.Never);

            _servicoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        [Fact]
        public async Task AdicionarAsync_ComNomeJaCadastrado_DeveRetornarFailure()
        {
            var dto = DtoValido();

            _servicoRepositorioMock
                .Setup(r => r.ExistePorNomeAsync(dto.Nome.Trim(), null))
                .ReturnsAsync(true);

            var resultado = await _service.AdicionarAsync(dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Esse serviço já está cadastrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.Validation,
                resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("123456")]
        [InlineData("999")]
        public async Task AdicionarAsync_ComNomeInvalido_DeveRetornarFailure(
            string nome)
        {
            var dto = DtoValido() with
            {
                Nome = nome
            };

            var resultado = await _service.AdicionarAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AdicionarAsync_ComPrecoInvalido_DeveRetornarFailure(
            decimal preco)
        {
            var dto = DtoValido() with
            {
                Preco = preco
            };

            var resultado = await _service.AdicionarAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        public async Task AdicionarAsync_ComTempoMedioInvalido_DeveRetornarFailure(
            int tempoMedio)
        {
            var dto = DtoValido() with
            {
                TempoMedio = tempoMedio
            };

            var resultado = await _service.AdicionarAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        // ============================================================
        // ATUALIZAR
        // ============================================================

        [Fact]
        public async Task AtualizarAsync_ComServicoExistenteEDadosValidos_DeveAtualizarERetornarSucesso()
        {
            var servico = ServicoValido();

            var dto = new AtualizaServicoDto(
                "Alinhamento",
                200m,
                90);

            _servicoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(servico.Id))
                .ReturnsAsync(servico);

            _servicoRepositorioMock
                .Setup(r => r.ExistePorNomeAsync(dto.Nome.Trim(), servico.Id))
                .ReturnsAsync(false);

            var resultado = await _service.AtualizarAsync(
                servico.Id,
                dto);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(dto.Nome, resultado.Value!.Nome);
            Assert.Equal(dto.Preco, resultado.Value.Preco);
            Assert.Equal(dto.TempoMedio, resultado.Value.TempoMedio);

            _servicoRepositorioMock.Verify(
                r => r.ObterPorIdAsync(servico.Id),
                Times.Once);

            _servicoRepositorioMock.Verify(
                r => r.ExistePorNomeAsync(dto.Nome.Trim(), servico.Id),
                Times.Once);

            _servicoRepositorioMock.Verify(
                r => r.AtualizarAsync(servico),
                Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComDadosInvalidos_DeveRetornarFailureSemBuscarServico()
        {
            var dto = new AtualizaServicoDto(
                "",
                200m,
                90);

            var resultado = await _service.AtualizarAsync(1, dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.ObterPorIdAsync(It.IsAny<int>()),
                Times.Never);

            _servicoRepositorioMock.Verify(
                r => r.AtualizarAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComServicoInexistente_DeveRetornarFailureNotFound()
        {
            var dto = new AtualizaServicoDto(
                "Alinhamento",
                200m,
                90);

            _servicoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Servico?)null);

            var resultado = await _service.AtualizarAsync(1, dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Serviço não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.AtualizarAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComNomeJaCadastradoEmOutroServico_DeveRetornarFailure()
        {
            var servico = ServicoValido();

            var dto = new AtualizaServicoDto(
                "Alinhamento",
                200m,
                90);

            _servicoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(servico.Id))
                .ReturnsAsync(servico);

            _servicoRepositorioMock
                .Setup(r => r.ExistePorNomeAsync(dto.Nome.Trim(), servico.Id))
                .ReturnsAsync(true);

            var resultado = await _service.AtualizarAsync(
                servico.Id,
                dto);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Esse serviço já está cadastrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.Validation,
                resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.AtualizarAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        // ============================================================
        // EXCLUIR
        // ============================================================

        [Fact]
        public async Task ExcluirAsync_ComServicoExistente_DeveExcluirERetornarSucesso()
        {
            var servico = ServicoValido();

            _servicoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(servico.Id))
                .ReturnsAsync(servico);

            var resultado = await _service.ExcluirAsync(servico.Id);

            Assert.True(resultado.IsSuccess);

            _servicoRepositorioMock.Verify(
                r => r.ExcluirAsync(servico),
                Times.Once);
        }

        [Fact]
        public async Task ExcluirAsync_ComServicoInexistente_DeveRetornarFailureNotFound()
        {
            _servicoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Servico?)null);

            var resultado = await _service.ExcluirAsync(1);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Serviço não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);

            _servicoRepositorioMock.Verify(
                r => r.ExcluirAsync(It.IsAny<Servico>()),
                Times.Never);
        }

        // ============================================================
        // OBTER POR ID
        // ============================================================

        [Fact]
        public async Task ObterPorIdAsync_ComServicoExistente_DeveRetornarSucessoComDto()
        {
            var servico = ServicoValido();

            _servicoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(servico.Id))
                .ReturnsAsync(servico);

            var resultado = await _service.ObterPorIdAsync(servico.Id);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(servico.Id, resultado.Value!.Id);
            Assert.Equal(servico.Nome, resultado.Value.Nome);
            Assert.Equal(servico.Preco, resultado.Value.Preco);
            Assert.Equal(servico.TempoMedio, resultado.Value.TempoMedio);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComServicoInexistente_DeveRetornarFailureNotFound()
        {
            _servicoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Servico?)null);

            var resultado = await _service.ObterPorIdAsync(1);

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Serviço não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);
        }

        // ============================================================
        // OBTER POR NOME
        // ============================================================

        [Fact]
        public async Task ObterPorNomeAsync_ComServicoExistente_DeveRetornarSucessoComDto()
        {
            var servico = ServicoValido();

            _servicoRepositorioMock
                .Setup(r => r.ObterPorNomeAsync(servico.Nome))
                .ReturnsAsync(servico);

            var resultado = await _service.ObterPorNomeAsync(
                servico.Nome);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(servico.Id, resultado.Value!.Id);
            Assert.Equal(servico.Nome, resultado.Value.Nome);
            Assert.Equal(servico.Preco, resultado.Value.Preco);
            Assert.Equal(servico.TempoMedio, resultado.Value.TempoMedio);
        }

        [Fact]
        public async Task ObterPorNomeAsync_ComServicoInexistente_DeveRetornarFailureNotFound()
        {
            _servicoRepositorioMock
                .Setup(r => r.ObterPorNomeAsync(It.IsAny<string>()))
                .ReturnsAsync((Servico?)null);

            var resultado = await _service.ObterPorNomeAsync(
                "Serviço inexistente");

            Assert.False(resultado.IsSuccess);

            Assert.Equal(
                "Serviço não encontrado.",
                resultado.Error);

            Assert.Equal(
                ErrorType.NotFound,
                resultado.ErrorType);
        }

        // ============================================================
        // OBTER TODOS
        // ============================================================

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodosOsServicosComoDto()
        {
            var servicos = new List<Servico>
            {
                ServicoValido(1),
                ServicoValido(2)
            };

            _servicoRepositorioMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(servicos);

            var resultado = (await _service.ObterTodosAsync()).ToList();

            Assert.Equal(2, resultado.Count);

            Assert.Equal(
                servicos[0].Id,
                resultado[0].Id);

            Assert.Equal(
                servicos[1].Id,
                resultado[1].Id);
        }

        [Fact]
        public async Task ObterTodosAsync_SemServicos_DeveRetornarListaVazia()
        {
            _servicoRepositorioMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync([]);

            var resultado = await _service.ObterTodosAsync();

            Assert.Empty(resultado);
        }
    }
}