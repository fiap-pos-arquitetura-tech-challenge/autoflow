using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;
using AutoFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.UnitTests.Application.Validators
{
    public class VeiculoValidadorTests
    {
        private static CriaVeiculoDto DtoValido() =>
        new(
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

        [Fact]
        public void Validar_ComDadosValidos_DeveRetornarNull()
        {
            var resultado = VeiculoValidator.Validar(DtoValido());

            Assert.Null(resultado);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validar_ComClienteIdInvalido_DeveRetornarErro(int clienteId)
        {
            var dto = DtoValido() with { ClienteId = clienteId };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("O Id do cliente é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validar_ComMarcaInvalida_DeveRetornarErro(string? marca)
        {
            var dto = DtoValido() with { Marca = marca! };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Marca é obrigatória.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validar_ComModeloInvalido_DeveRetornarErro(string? modelo)
        {
            var dto = DtoValido() with { Modelo = modelo! };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Modelo é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(1900)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validar_ComAnoFabricacaoInvalido_DeveRetornarErro(int ano)
        {
            var dto = DtoValido() with { AnoFabricacao = ano };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Ano de fabricação é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(1900)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validar_ComAnoModeloInvalido_DeveRetornarErro(int ano)
        {
            var dto = DtoValido() with { AnoModelo = ano };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Ano do modelo é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validar_ComCorInvalida_DeveRetornarErro(string? cor)
        {
            var dto = DtoValido() with { Cor = cor! };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Cor é obrigatória.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validar_ComPlacaInvalida_DeveRetornarErro(string? placa)
        {
            var dto = DtoValido() with { Placa = placa! };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Placa é obrigatória.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validar_ComChassiInvalido_DeveRetornarErro(string? chassi)
        {
            var dto = DtoValido() with { Chassi = chassi! };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Chassi é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Validar_ComQuilometragemNegativa_DeveRetornarErro(int quilometragem)
        {
            var dto = DtoValido() with { Quilometragem = quilometragem };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Quilometragem não pode ser negativa.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_ComTipoVeiculoInvalido_DeveRetornarErro()
        {
            var dto = DtoValido() with
            {
                Tipo = (TipoVeiculo)999
            };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Tipo de veículo é inválido.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_ComCombustivelInvalido_DeveRetornarErro()
        {
            var dto = DtoValido() with
            {
                Combustivel = (Combustivel)999
            };

            var resultado = VeiculoValidator.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Tipo de combustivél deve ser fornecido.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }
    }
}
