using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Models;
using AutoFlow.Domain.ValueObjects;
using Moq;

namespace AutoFlow.UnitTests.Application.Services
{
    public class OrdemServicoServiceTests
    {
        private readonly Mock<IOrdemServicoRepositorio> _ordemServicoRepositorioMock = new();
        private readonly Mock<IClienteRepositorio> _clienteRepositorioMock = new();
        private readonly Mock<IVeiculoRepositorio> _veiculoRepositorioMock = new();
        private readonly Mock<IServicoRepositorio> _servicoRepositorioMock = new();
        private readonly Mock<IPecaInsumoRepositorio> _pecaInsumoRepositorioMock = new();
        private readonly Mock<IEstoqueRepositorio> _estoqueRepositorioMock = new();
        private readonly OrdemServicoService _service;

        public OrdemServicoServiceTests()
        {
            _service = new OrdemServicoService(_ordemServicoRepositorioMock.Object, _clienteRepositorioMock.Object, _veiculoRepositorioMock.Object, _servicoRepositorioMock.Object,
                _pecaInsumoRepositorioMock.Object, _estoqueRepositorioMock.Object);
        }

        [Fact]
        public async Task AdicionarAsync_ComClienteEVeiculoValidos_DeveAdicionarOrdem()
        {
            var cliente = CriarCliente();
            var veiculo = CriarVeiculo();

            _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(cliente);
            _veiculoRepositorioMock.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(veiculo);

            var resultado = await _service.AdicionarAsync(
                new CriaOrdemServicoDto(1, 2, "Risco no para-choque"));

            Assert.True(resultado.IsSuccess);
            Assert.Equal(StatusOrdemServico.Recebida, resultado.Value!.Status);
            Assert.Equal(1, resultado.Value.ClienteId);
            Assert.Equal(2, resultado.Value.VeiculoId);

            _ordemServicoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<OrdemServico>()),
                Times.Once);
        }

        [Fact]
        public async Task AdicionarAsync_ComVeiculoDeOutroCliente_DeveRetornarFailure()
        {
            var cliente = CriarCliente();
            var veiculo = CriarVeiculo(clienteId: 99);

            _clienteRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(cliente);
            _veiculoRepositorioMock.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(veiculo);

            var resultado = await _service.AdicionarAsync(
                new CriaOrdemServicoDto(1, 2, null));

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _ordemServicoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<OrdemServico>()),
                Times.Never);
        }

        [Fact]
        public async Task IniciarDiagnosticoAsync_ComOrdemExistente_DeveAtualizarStatus()
        {
            var ordem = new OrdemServico(1, 2) { Id = 10 };
            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(10)).ReturnsAsync(ordem);

            var resultado = await _service.IniciarDiagnosticoAsync(10);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(StatusOrdemServico.EmDiagnostico, resultado.Value!.Status);
            _ordemServicoRepositorioMock.Verify(r => r.AtualizarAsync(ordem), Times.Once);
        }

        [Fact]
        public async Task AdicionarServicoAsync_ComServicoExistente_DeveUsarSnapshotDoCatalogo()
        {
            var ordem = CriarOrdemEmDiagnostico();
            var servico = new Servico("Troca de óleo", 250m, 60) { Id = 5 };

            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(1)).ReturnsAsync(ordem);
            _servicoRepositorioMock.Setup(r => r.ObterPorIdAsync(5)).ReturnsAsync(servico);

            var resultado = await _service.AdicionarServicoAsync(
                1,
                new AdicionaServicoOrdemServicoDto(5, 2));

            Assert.True(resultado.IsSuccess);
            var item = Assert.Single(resultado.Value!.Servicos);
            Assert.Equal("Troca de óleo", item.Descricao);
            Assert.Equal(250m, item.ValorUnitario);
            Assert.Equal(60, item.TempoPrevisto);
            Assert.Equal(500m, item.Subtotal);
        }

        [Fact]
        public async Task AdicionarPecaAsync_ComEstoqueInsuficiente_DeveRetornarFailure()
        {
            var ordem = CriarOrdemEmDiagnostico();
            var peca = new PecaInsumo("Filtro de óleo", 50m) { Id = 7 };
            var estoque = new Estoque(peca, 1) { Id = 3 };

            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(1)).ReturnsAsync(ordem);
            _pecaInsumoRepositorioMock.Setup(r => r.ObterPorIdAsync(7)).ReturnsAsync(peca);
            _estoqueRepositorioMock.Setup(r => r.ObterPorPecaInsumoAsync(7)).ReturnsAsync(estoque);

            var resultado = await _service.AdicionarPecaAsync(
                1,
                new AdicionaPecaOrdemServicoDto(7, 2));

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
            Assert.Contains("Estoque insuficiente", resultado.Error);

            _ordemServicoRepositorioMock.Verify(
                r => r.AtualizarAsync(It.IsAny<OrdemServico>()),
                Times.Never);
        }

        [Fact]
        public async Task AdicionarPecaAsync_ComEstoqueSuficiente_DeveAdicionarSnapshot()
        {
            var ordem = CriarOrdemEmDiagnostico();
            var peca = new PecaInsumo("Filtro de óleo", 50m) { Id = 7 };
            var estoque = new Estoque(peca, 10) { Id = 3 };

            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(1)).ReturnsAsync(ordem);
            _pecaInsumoRepositorioMock.Setup(r => r.ObterPorIdAsync(7)).ReturnsAsync(peca);
            _estoqueRepositorioMock.Setup(r => r.ObterPorPecaInsumoAsync(7)).ReturnsAsync(estoque);

            var resultado = await _service.AdicionarPecaAsync(
                1,
                new AdicionaPecaOrdemServicoDto(7, 2));

            Assert.True(resultado.IsSuccess);
            var item = Assert.Single(resultado.Value!.Pecas);
            Assert.Equal("Filtro de óleo", item.Descricao);
            Assert.Equal(50m, item.ValorUnitario);
            Assert.Equal(100m, item.Subtotal);
        }

        [Fact]
        public async Task GerarOrcamentoAsync_ComOrdemPreparada_DeveRetornarTotalCalculado()
        {
            var ordem = CriarOrdemEmDiagnostico();
            ordem.AdicionarServico(1, "Troca de óleo", 1, 200m, 60);
            ordem.AdicionarPeca(2, "Filtro", 1, 50m);

            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(1)).ReturnsAsync(ordem);

            var resultado = await _service.GerarOrcamentoAsync(1);

            Assert.True(resultado.IsSuccess);
            Assert.NotNull(resultado.Value!.Orcamento);
            Assert.Equal(250m, resultado.Value.Orcamento!.ValorTotal);
            Assert.Equal(StatusOrdemServico.AguardandoAprovacao, resultado.Value.Status);
        }

        [Fact]
        public async Task AprovarOrcamentoAsync_ComEstoqueDisponivel_DeveIniciarExecucao()
        {
            var ordem = CriarOrdemAguardandoAprovacao();
            var peca = new PecaInsumo("Filtro", 50m) { Id = 2 };
            var estoque = new Estoque(peca, 5);

            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(1)).ReturnsAsync(ordem);
            _estoqueRepositorioMock.Setup(r => r.ObterPorPecaInsumoAsync(2)).ReturnsAsync(estoque);

            var resultado = await _service.AprovarOrcamentoAsync(1);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(StatusOrdemServico.EmExecucao, resultado.Value!.Status);
            Assert.Equal(StatusOrcamento.Aprovado, resultado.Value.Orcamento!.Status);
            Assert.Equal(4, estoque.Quantidade.Valor);
        }


        [Fact]
        public async Task AdicionarPecaAsync_ComMesmaPecaJaAdicionada_DeveValidarQuantidadeTotalDaOs()
        {
            var ordem = CriarOrdemEmDiagnostico();
            var peca = new PecaInsumo("Filtro de óleo", 50m) { Id = 7 };
            var estoque = new Estoque(peca, 5) { Id = 3 };
            ordem.AdicionarPeca(7, "Filtro de óleo", 3, 50m);

            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(1)).ReturnsAsync(ordem);
            _pecaInsumoRepositorioMock.Setup(r => r.ObterPorIdAsync(7)).ReturnsAsync(peca);
            _estoqueRepositorioMock.Setup(r => r.ObterPorPecaInsumoAsync(7)).ReturnsAsync(estoque);

            var resultado = await _service.AdicionarPecaAsync(
                1,
                new AdicionaPecaOrdemServicoDto(7, 3));

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
            Assert.Contains("Solicitado no total da OS: 6", resultado.Error);
        }

        [Fact]
        public async Task AprovarOrcamentoAsync_ComPecaRepetida_DeveBaixarQuantidadeTotalUmaUnicaVez()
        {
            var ordem = CriarOrdemEmDiagnostico();
            ordem.AdicionarPeca(2, "Filtro", 2, 50m);
            ordem.AdicionarPeca(2, "Filtro", 3, 50m);
            ordem.GerarOrcamento();

            var peca = new PecaInsumo("Filtro", 50m) { Id = 2 };
            var estoque = new Estoque(peca, 10);

            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(1)).ReturnsAsync(ordem);
            _estoqueRepositorioMock.Setup(r => r.ObterPorPecaInsumoAsync(2)).ReturnsAsync(estoque);

            var resultado = await _service.AprovarOrcamentoAsync(1);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(5, estoque.Quantidade.Valor);
            _estoqueRepositorioMock.Verify(r => r.ObterPorPecaInsumoAsync(2), Times.Once);
        }

        [Fact]
        public async Task IniciarEFinalizarExecucaoServicoAsync_DeveAtualizarDatasDoItem()
        {
            var ordem = CriarOrdemAguardandoAprovacao();
            var item = Assert.Single(ordem.Servicos);
            item.Id = 30;
            ordem.AprovarOrcamento();

            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(1)).ReturnsAsync(ordem);

            var inicio = await _service.IniciarExecucaoServicoAsync(1, 30);
            var fim = await _service.FinalizarExecucaoServicoAsync(1, 30);

            Assert.True(inicio.IsSuccess);
            Assert.True(fim.IsSuccess);
            Assert.NotNull(item.ExecucaoIniciadaEm);
            Assert.NotNull(item.ExecucaoFinalizadaEm);
        }

        [Fact]
        public async Task ReprovarOrcamentoAsync_SemJustificativa_DeveRetornarValidation()
        {
            var resultado = await _service.ReprovarOrcamentoAsync(
                1,
                new ReprovaOrcamentoOrdemServicoDto(""));

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _ordemServicoRepositorioMock.Verify(
                r => r.ObterCompletaPorIdAsync(It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task ConsultarAndamentoAsync_ComOrdemExistente_DeveRetornarStatus()
        {
            var ordem = new OrdemServico(1, 2) { Id = 15 };
            _ordemServicoRepositorioMock.Setup(r => r.ObterCompletaPorIdAsync(15)).ReturnsAsync(ordem);

            var resultado = await _service.ConsultarAndamentoAsync(15);

            Assert.True(resultado.IsSuccess);
            Assert.Equal(15, resultado.Value!.Id);
            Assert.Equal(StatusOrdemServico.Recebida, resultado.Value.Status);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComOrdemInexistente_DeveRetornarNotFound()
        {
            _ordemServicoRepositorioMock
                .Setup(r => r.ObterCompletaPorIdAsync(99))
                .ReturnsAsync((OrdemServico?)null);

            var resultado = await _service.ObterPorIdAsync(99);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);
        }

        private static Cliente CriarCliente()
        {
            return new Cliente(
                "Cliente Teste",
                "52998224725",
                "11999999999",
                "cliente@teste.com")
            {
                Id = 1
            };
        }

        private static Veiculo CriarVeiculo(int clienteId = 1)
        {
            return new Veiculo(
                clienteId,
                "Toyota",
                "Corolla",
                2020,
                2021,
                "Preto",
                TipoVeiculo.Carro,
                Combustivel.Gasolina,
                new Placa("ABC1D23"),
                new Chassi("9BWZZZ377VT004251"),
                new Quilometragem(10000))
            {
                Id = 2
            };
        }

        private static OrdemServico CriarOrdemEmDiagnostico()
        {
            var ordem = new OrdemServico(1, 2) { Id = 1 };
            ordem.IniciarDiagnostico();
            ordem.RegistrarDiagnostico("Troca de óleo e filtro necessária.");
            return ordem;
        }

        private static OrdemServico CriarOrdemAguardandoAprovacao()
        {
            var ordem = CriarOrdemEmDiagnostico();
            ordem.AdicionarServico(1, "Troca de óleo", 1, 200m, 60);
            ordem.AdicionarPeca(2, "Filtro", 1, 50m);
            ordem.GerarOrcamento();
            return ordem;
        }
    }
}
