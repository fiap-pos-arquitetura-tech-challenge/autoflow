using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Models;
using AutoFlow.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.UnitTests.Application.Services
{
    public class VeiculoServiceTests
    {
        private readonly Mock<IVeiculoRepositorio> _veiculoRepositorioMock = new();
        private readonly VeiculoService _service;

        public VeiculoServiceTests()
        {
            _service = new VeiculoService(_veiculoRepositorioMock.Object);
        }

        private static Veiculo CriarVeiculoValido(int id = 1)
        {
            return new Veiculo(
                clienteId: 1,
                marca: "Toyota",
                modelo: "Corolla",
                anoFabricacao: 2020,
                anoModelo: 2021,
                cor: "Preto",
                tipo: TipoVeiculo.Carro,
                combustivel: Combustivel.Gasolina,
                placa: new Placa("ABC1D23"),
                chassi: new Chassi("9BWZZZ377VT004251"),
                quilometragem: new Quilometragem(10000)
            )
            {
                Id = id
            };
        }

        private static CriaVeiculoDto CriarDtoValido()
        {
            return new CriaVeiculoDto(
                ClienteId: 1,
                Marca: "Toyota",
                Modelo: "Corolla",
                AnoFabricacao: 2020,
                AnoModelo: 2021,
                Cor: "Preto",
                Tipo: TipoVeiculo.Carro,
                Combustivel: Combustivel.Gasolina,
                Placa: "ABC1D23",
                Chassi: "9BWZZZ377VT004251",
                Quilometragem: 10000
            );
        }

        [Fact]
        public async Task AdicionarAsync_ComDadosValidos_DeveAdicionarERetornarSucesso()
        {
            var dto = CriarDtoValido();

            var resultado = await _service.AdicionarAsync(dto);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(dto.ClienteId, resultado.Value!.ClienteID);
            Assert.Equal(dto.Marca, resultado.Value.Marca);
            Assert.Equal(dto.Modelo, resultado.Value.Modelo);
            Assert.Equal(dto.AnoFabricacao, resultado.Value.AnoFabricacao);
            Assert.Equal(dto.AnoModelo, resultado.Value.AnoModelo);
            Assert.Equal(dto.Cor, resultado.Value.Cor);
            Assert.Equal(dto.Tipo, resultado.Value.Tipo);
            Assert.Equal(dto.Combustivel, resultado.Value.Combustivel);
            Assert.Equal(dto.Placa, resultado.Value.Placa);
            Assert.Equal(dto.Chassi, resultado.Value.Chassi);
            Assert.Equal(dto.Quilometragem, resultado.Value.Quilometragem);

            _veiculoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Veiculo>()),
                Times.Once);
        }

        [Fact]
        public async Task AdicionarAsync_ComDadosInvalidos_DeveRetornarFailure()
        {
            var dto = CriarDtoValido() with
            {
                Marca = ""
            };

            var resultado = await _service.AdicionarAsync(dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _veiculoRepositorioMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Veiculo>()),
                Times.Never);
        }

        [Fact]
        public async Task AdicionarAsync_DeveMapearCorretamenteParaEntidade()
        {
            var dto = CriarDtoValido();

            await _service.AdicionarAsync(dto);

            _veiculoRepositorioMock.Verify(
                r => r.AdicionarAsync(
                    It.Is<Veiculo>(v =>
                        v.ClienteId == dto.ClienteId &&
                        v.Marca == dto.Marca &&
                        v.Modelo == dto.Modelo &&
                        v.AnoFabricacao == dto.AnoFabricacao &&
                        v.AnoModelo == dto.AnoModelo &&
                        v.Cor == dto.Cor &&
                        v.Tipo == dto.Tipo &&
                        v.Combustivel == dto.Combustivel &&
                        v.Placa.Valor == dto.Placa &&
                        v.Chassi.Valor == dto.Chassi &&
                        v.Quilometragem.Valor == dto.Quilometragem)),
                Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComVeiculoExistente_DeveAtualizarERetornarSucesso()
        {
            var veiculo = CriarVeiculoValido();

            var dto = new AtualizaVeiculoDto(
                ClienteId: 2,
                Marca: "Honda",
                Modelo: "Civic",
                AnoFabricacao: 2022,
                AnoModelo: 2023,
                Cor: "Branco",
                Tipo: TipoVeiculo.Carro,
                Combustivel: Combustivel.Etanol,
                Placa: "XYZ9A99",
                Chassi: "9BWZZZ377VT004252",
                Quilometragem: 20000
            );

            _veiculoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(veiculo.Id))
                .ReturnsAsync(veiculo);

            var resultado = await _service.AtualizarAsync(veiculo.Id, dto);

            Assert.True(resultado.IsSuccess);

            _veiculoRepositorioMock.Verify(
                r => r.AtualizarAsync(veiculo),
                Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComDadosInvalidos_DeveRetornarFailure()
        {
            var dto = new AtualizaVeiculoDto(
                ClienteId: 0,
                Marca: "Honda",
                Modelo: "Civic",
                AnoFabricacao: 2022,
                AnoModelo: 2023,
                Cor: "Branco",
                Tipo: TipoVeiculo.Carro,
                Combustivel: Combustivel.Etanol,
                Placa: "XYZ9A99",
                Chassi: "9BWZZZ377VT004252",
                Quilometragem: 20000
            );

            var resultado = await _service.AtualizarAsync(1, dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);

            _veiculoRepositorioMock.Verify(
                r => r.ObterPorIdAsync(It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComVeiculoInexistente_DeveRetornarNotFound()
        {
            var dto = new AtualizaVeiculoDto(
                1,
                "Honda",
                "Civic",
                2022,
                2023,
                "Branco",
                TipoVeiculo.Carro,
                Combustivel.Etanol,
                "XYZ9A99",
                "9BWZZZ377VT004252",
                20000
            );

            _veiculoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Veiculo?)null);

            var resultado = await _service.AtualizarAsync(1, dto);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);
            Assert.Equal("Veiculo não encontrado!", resultado.Error);

            _veiculoRepositorioMock.Verify(
                r => r.AtualizarAsync(It.IsAny<Veiculo>()),
                Times.Never);
        }

        [Fact]
        public async Task ExcluirAsync_ComVeiculoExistente_DeveExcluir()
        {
            var veiculo = CriarVeiculoValido();

            _veiculoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(veiculo.Id))
                .ReturnsAsync(veiculo);

            var resultado = await _service.ExcluirAsync(veiculo.Id);

            Assert.True(resultado.IsSuccess);

            _veiculoRepositorioMock.Verify(
                r => r.ExcluirAsync(veiculo),
                Times.Once);
        }

        [Fact]
        public async Task ExcluirAsync_ComVeiculoInexistente_DeveRetornarNotFound()
        {
            _veiculoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Veiculo?)null);

            var resultado = await _service.ExcluirAsync(1);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);

            _veiculoRepositorioMock.Verify(
                r => r.ExcluirAsync(It.IsAny<Veiculo>()),
                Times.Never);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComVeiculoExistente_DeveRetornarDto()
        {
            var veiculo = CriarVeiculoValido();

            _veiculoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(veiculo.Id))
                .ReturnsAsync(veiculo);

            var resultado = await _service.ObterPorIdAsync(veiculo.Id);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(veiculo.Id, resultado.Value!.Id);
            Assert.Equal(veiculo.ClienteId, resultado.Value.ClienteID);
            Assert.Equal(veiculo.Marca, resultado.Value.Marca);
            Assert.Equal(veiculo.Modelo, resultado.Value.Modelo);
            Assert.Equal(veiculo.Placa.Valor, resultado.Value.Placa);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComVeiculoInexistente_DeveRetornarNotFound()
        {
            _veiculoRepositorioMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Veiculo?)null);

            var resultado = await _service.ObterPorIdAsync(1);

            Assert.False(resultado.IsSuccess);
            Assert.Equal(ErrorType.NotFound, resultado.ErrorType);
        }

        [Fact]
        public async Task ObterTodosAsync_ComVeiculos_DeveRetornarLista()
        {
            var veiculos = new List<Veiculo>
        {
            CriarVeiculoValido(1),
            CriarVeiculoValido(2)
        };

            _veiculoRepositorioMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(veiculos);

            var resultado = (await _service.ObterTodosAsync()).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Equal(1, resultado[0].Id);
            Assert.Equal(2, resultado[1].Id);
        }

        [Fact]
        public async Task ObterTodosAsync_SemVeiculos_DeveRetornarListaVazia()
        {
            _veiculoRepositorioMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(new List<Veiculo>());

            var resultado = await _service.ObterTodosAsync();

            Assert.Empty(resultado);
        }
    }
}
