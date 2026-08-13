using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;
using AutoFlow.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class VeiculoTests
    {
        private static Veiculo CriarVeiculoValido()
        {
            return new Veiculo(
                clienteId: 1,
                marca: "Toyota",
                modelo: "Corolla",
                anoFabricacao: 2020,
                anoModelo: 2021,
                cor: "Prata",
                tipo: TipoVeiculo.Carro,
                combustivel: Combustivel.Flex,
                placa: new Placa("ABC1234"),
                chassi: new Chassi("9BWZZZ377VT004251"),
                quilometragem: new Quilometragem(50000));
        }

        [Fact]
        public void ConstrutorVazio_DeveCriarVeiculo()
        {
           
            var veiculo = new Veiculo();
            
            Assert.NotNull(veiculo);
        }

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarVeiculo()
        {
            var placa = new Placa("ABC1234");
            var chassi = new Chassi("9BWZZZ377VT004251");
            var quilometragem = new Quilometragem(50000);
            
            var veiculo = new Veiculo(
                clienteId: 1,
                marca: "Toyota",
                modelo: "Corolla",
                anoFabricacao: 2020,
                anoModelo: 2021,
                cor: "Prata",
                tipo: TipoVeiculo.Carro,
                combustivel: Combustivel.Flex,
                placa: placa,
                chassi: chassi,
                quilometragem: quilometragem);

            Assert.Equal(1, veiculo.ClienteId);
            Assert.Equal("Toyota", veiculo.Marca);
            Assert.Equal("Corolla", veiculo.Modelo);
            Assert.Equal(2020, veiculo.AnoFabricacao);
            Assert.Equal(2021, veiculo.AnoModelo);
            Assert.Equal("Prata", veiculo.Cor);
            Assert.Equal(TipoVeiculo.Carro, veiculo.Tipo);
            Assert.Equal(Combustivel.Flex, veiculo.Combustivel);
            Assert.Same(placa, veiculo.Placa);
            Assert.Same(chassi, veiculo.Chassi);
            Assert.Same(quilometragem, veiculo.Quilometragem);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComMarcaInvalida_DeveLancarArgumentException(
            string? marca)
        {            
            var exception = Assert.Throws<ArgumentException>(() =>
                new Veiculo(
                    clienteId: 1,
                    marca: marca!,
                    modelo: "Corolla",
                    anoFabricacao: 2020,
                    anoModelo: 2021,
                    cor: "Prata",
                    tipo: TipoVeiculo.Carro,
                    combustivel: Combustivel.Flex,
                    placa: new Placa("ABC1234"),
                    chassi: new Chassi("9BWZZZ377VT004251"),
                    quilometragem: new Quilometragem(50000)));
            
            Assert.Equal("Marca é obrigatória.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComModeloInvalido_DeveLancarArgumentException(
            string? modelo)
        {            
            var exception = Assert.Throws<ArgumentException>(() =>
                new Veiculo(
                    clienteId: 1,
                    marca: "Toyota",
                    modelo: modelo!,
                    anoFabricacao: 2020,
                    anoModelo: 2021,
                    cor: "Prata",
                    tipo: TipoVeiculo.Carro,
                    combustivel: Combustivel.Flex,
                    placa: new Placa("ABC1234"),
                    chassi: new Chassi("9BWZZZ377VT004251"),
                    quilometragem: new Quilometragem(50000)));

            Assert.Equal("Modelo é obrigatório.", exception.Message);
        }

        [Theory]
        [InlineData(1899)]        
        [InlineData(1000)]
        public void Construtor_ComAnoFabricacaoInvalido_DeveLancarArgumentException(
            int anoFabricacao)
        {            
            var exception = Assert.Throws<ArgumentException>(() =>
                new Veiculo(
                    clienteId: 1,
                    marca: "Toyota",
                    modelo: "Corolla",
                    anoFabricacao: anoFabricacao,
                    anoModelo: 2021,
                    cor: "Prata",
                    tipo: TipoVeiculo.Carro,
                    combustivel: Combustivel.Flex,
                    placa: new Placa("ABC1234"),
                    chassi: new Chassi("9BWZZZ377VT004251"),
                    quilometragem: new Quilometragem(50000)));

            Assert.Equal("Ano de fabricação inválido.", exception.Message);
        }

        [Fact]
        public void Construtor_ComAnoFabricacao1900_DeveCriarVeiculo()
        {           
            var veiculo = new Veiculo(
                1,
                "Ford",
                "Ka",
                1900,
                1900,
                "Preto",
                TipoVeiculo.Carro,
                Combustivel.Flex,
                new Placa("ABC1234"),
                new Chassi("9BWZZZ377VT004251"),
                new Quilometragem(100));
            
            Assert.Equal(1900, veiculo.AnoFabricacao);
        }

        [Fact]
        public void AtualizarQuilometragem_ComValorMaior_DeveAtualizar()
        {            
            var veiculo = CriarVeiculoValido();
            
            veiculo.AtualizarQuilometragem(60000);
            
            Assert.Equal(60000, veiculo.Quilometragem.Valor);
        }

        [Fact]
        public void AtualizarQuilometragem_ComMesmoValor_DeveManterQuilometragem()
        {            
            var veiculo = CriarVeiculoValido();
            
            veiculo.AtualizarQuilometragem(50000);
            
            Assert.Equal(50000, veiculo.Quilometragem.Valor);
        }

        [Theory]
        [InlineData(49999)]
        [InlineData(40000)]
        [InlineData(0)]
        public void AtualizarQuilometragem_ComValorMenor_DeveLancarQuilometragemInvalidaException(
            int novaQuilometragem)
        {            
            var veiculo = CriarVeiculoValido();
            
            var exception = Assert.Throws<QuilometragemInvalidaException>(() =>
                veiculo.AtualizarQuilometragem(novaQuilometragem));
            
            Assert.Equal(
                "A quilometragem não pode diminuir.",
                exception.Message);

            Assert.Equal(50000, veiculo.Quilometragem.Valor);
        }

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarTodasAsPropriedades()
        {            
            var veiculo = CriarVeiculoValido();
            
            veiculo.Atualizar(
                clienteId: 2,
                marca: "Honda",
                modelo: "Civic",
                anoFabricacao: 2022,
                anoModelo: 2023,
                cor: "Preto",
                tipo: TipoVeiculo.Carro,
                combustivel: Combustivel.Flex,
                placa: "XYZ1A23",
                chassi: "8APZZZ377VT123456",
                quilometragem: 60000);
            
            Assert.Equal(2, veiculo.ClienteId);
            Assert.Equal("Honda", veiculo.Marca);
            Assert.Equal("Civic", veiculo.Modelo);
            Assert.Equal(2022, veiculo.AnoFabricacao);
            Assert.Equal(2023, veiculo.AnoModelo);
            Assert.Equal("Preto", veiculo.Cor);
            Assert.Equal(TipoVeiculo.Carro, veiculo.Tipo);
            Assert.Equal(Combustivel.Flex, veiculo.Combustivel);

            Assert.Equal("XYZ1A23", veiculo.Placa.Valor);
            Assert.Equal("8APZZZ377VT123456", veiculo.Chassi.Valor);
            Assert.Equal(60000, veiculo.Quilometragem.Valor);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Atualizar_ComMarcaInvalida_DeveLancarArgumentException(
            string? marca)
        {            
            var veiculo = CriarVeiculoValido();
            
            var exception = Assert.Throws<ArgumentException>(() =>
                veiculo.Atualizar(
                    2,
                    marca!,
                    "Civic",
                    2022,
                    2023,
                    "Preto",
                    TipoVeiculo.Carro,
                    Combustivel.Flex,
                    "XYZ1A23",
                    "8APZZZ377VT123456",
                    60000));
            
            Assert.Equal("Marca é obrigatória.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Atualizar_ComModeloInvalido_DeveLancarArgumentException(
            string? modelo)
        {           
            var veiculo = CriarVeiculoValido();
            
            var exception = Assert.Throws<ArgumentException>(() =>
                veiculo.Atualizar(
                    2,
                    "Honda",
                    modelo!,
                    2022,
                    2023,
                    "Preto",
                    TipoVeiculo.Moto,
                    Combustivel.Gasolina,
                    "XYZ1A23",
                    "8APZZZ377VT123456",
                    60000));
            
            Assert.Equal("Modelo é obrigatório.", exception.Message);
        }

        [Fact]
        public void Atualizar_ComAnoFabricacaoInvalido_DeveLancarArgumentException()
        {            
            var veiculo = CriarVeiculoValido();
            
            var exception = Assert.Throws<ArgumentException>(() =>
                veiculo.Atualizar(
                    2,
                    "Honda",
                    "Civic",
                    1899,
                    2023,
                    "Preto",
                    TipoVeiculo.Carro,
                    Combustivel.Flex,
                    "XYZ1A23",
                    "8APZZZ377VT123456",
                    60000));
            
            Assert.Equal("Ano de fabricação inválido.", exception.Message);
        }

        [Fact]
        public void Atualizar_ComPlacaInvalida_DeveLancarPlacaInvalidaException()
        {            
            var veiculo = CriarVeiculoValido();
            
            var exception = Assert.Throws<PlacaInvalidaException>(() =>
                veiculo.Atualizar(
                    2,
                    "Honda",
                    "Civic",
                    2022,
                    2023,
                    "Preto",
                    TipoVeiculo.Carro,
                    Combustivel.Flex,
                    "INVALIDA",
                    "8APZZZ377VT123456",
                    60000));
            
            Assert.Equal("Formato de placa inválido.", exception.Message);
        }

        [Fact]
        public void Atualizar_ComChassiInvalido_DeveLancarChassiInvalidoException()
        {            
            var veiculo = CriarVeiculoValido();
            
            var exception = Assert.Throws<ChassiInvalidoException>(() =>
                veiculo.Atualizar(
                    2,
                    "Honda",
                    "Civic",
                    2022,
                    2023,
                    "Preto",
                    TipoVeiculo.Carro,
                    Combustivel.Flex,
                    "XYZ1A23",
                    "123",
                    60000));
            
            Assert.Equal(
                "Chassi deve possuir 17 caracteres.",
                exception.Message);
        }

        [Fact]
        public void Atualizar_ComQuilometragemInvalida_DeveLancarQuilometragemInvalidaException()
        {            
            var veiculo = CriarVeiculoValido();
            
            var exception = Assert.Throws<QuilometragemInvalidaException>(() =>
                veiculo.Atualizar(
                    2,
                    "Honda",
                    "Civic",
                    2022,
                    2023,
                    "Preto",
                    TipoVeiculo.Carro,
                    Combustivel.Flex,
                    "XYZ1A23",
                    "8APZZZ377VT123456",
                    -1));
            
            Assert.Equal("Quilometragem inválida.", exception.Message);
        }
    }
}
