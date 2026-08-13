using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class QuilometragemTests
    {
        [Fact]
        public void Construtor_ComQuilometragemPositiva_DeveCriarQuilometragem()
        {            
            var valor = 50000;
            
            var quilometragem = new Quilometragem(valor);
            
            Assert.Equal(50000, quilometragem.Valor);
        }

        [Fact]
        public void Construtor_ComQuilometragemZero_DeveCriarQuilometragem()
        {            
            var valor = 0;
            
            var quilometragem = new Quilometragem(valor);
            
            Assert.Equal(0, quilometragem.Valor);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(-50000)]
        [InlineData(int.MinValue)]
        public void Construtor_ComQuilometragemNegativa_DeveLancarQuilometragemInvalidaException(
            int valor)
        {            
            var exception = Assert.Throws<QuilometragemInvalidaException>(
                () => new Quilometragem(valor));
            
            Assert.Equal("Quilometragem inválida.", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(50000)]
        [InlineData(int.MaxValue)]
        public void Construtor_ComQuilometragemValida_DeveArmazenarValorCorretamente(
            int valor)
        {            
            var quilometragem = new Quilometragem(valor);
            
            Assert.Equal(valor, quilometragem.Valor);
        }

        [Fact]
        public void ToString_DeveRetornarValorComUnidadeKm()
        {            
            var quilometragem = new Quilometragem(50000);
            
            var resultado = quilometragem.ToString();
            
            Assert.Equal("50000 km", resultado);
        }

        [Theory]
        [InlineData(0, "0 km")]
        [InlineData(1, "1 km")]
        [InlineData(1000, "1000 km")]
        [InlineData(150000, "150000 km")]
        public void ToString_ComDiferentesQuilometragens_DeveRetornarFormatoCorreto(
            int valor,
            string esperado)
        {            
            var quilometragem = new Quilometragem(valor);
            
            var resultado = quilometragem.ToString();
            
            Assert.Equal(esperado, resultado);
        }
    }
}
