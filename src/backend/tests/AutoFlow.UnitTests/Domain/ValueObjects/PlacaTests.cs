using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class PlacaTests
    {
        [Fact]
        public void Construtor_ComPlacaAntigaValida_DeveCriarPlaca()
        {            
            var valor = "ABC1234";
            
            var placa = new Placa(valor);
            
            Assert.Equal("ABC1234", placa.Valor);
        }

        [Fact]
        public void Construtor_ComPlacaMercosulValida_DeveCriarPlaca()
        {            
            var valor = "ABC1D23";
            
            var placa = new Placa(valor);
            
            Assert.Equal("ABC1D23", placa.Valor);
        }

        [Fact]
        public void Construtor_ComLetrasMinusculas_DeveConverterParaMaiusculas()
        {            
            var valor = "abc1234";
            
            var placa = new Placa(valor);
            
            Assert.Equal("ABC1234", placa.Valor);
        }

        [Fact]
        public void Construtor_ComEspacosNasExtremidades_DeveRemoverEspacos()
        {            
            var valor = "  abc1234  ";
            
            var placa = new Placa(valor);
            
            Assert.Equal("ABC1234", placa.Valor);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComValorNuloOuVazio_DeveLancarPlacaInvalidaException(string? valor)
        {            
            var exception = Assert.Throws<PlacaInvalidaException>(
                () => new Placa(valor!));
            
            Assert.Equal("Placa inválida.", exception.Message);
        }

        [Theory]
        [InlineData("1234567")]
        [InlineData("AB12345")]
        [InlineData("ABCD123")]
        [InlineData("ABC12D3")]
        [InlineData("AAA-1234")]
        [InlineData("ABC1D234")]
        [InlineData("AB1D23")]
        public void Construtor_ComFormatoInvalido_DeveLancarPlacaInvalidaException(string valor)
        {            
            var exception = Assert.Throws<PlacaInvalidaException>(
                () => new Placa(valor));
            
            Assert.Equal("Formato de placa inválido.", exception.Message);
        }

        [Fact]
        public void ToString_DeveRetornarValorDaPlaca()
        {
            
            var placa = new Placa("ABC1234");

            
            var resultado = placa.ToString();

            
            Assert.Equal("ABC1234", resultado);
        }
    }
}
