using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class ChassiTests
    {
        [Fact]
        public void Construtor_ComChassiValido_DeveCriarChassi()
        {            
            var valor = "9BWZZZ377VT004251";
            
            var chassi = new Chassi(valor);
            
            Assert.Equal(valor, chassi.Valor);
        }

        [Fact]
        public void Construtor_ComLetrasMinusculas_DeveConverterParaMaiusculas()
        {            
            var valor = "abc123def456ghi78";
            
            var chassi = new Chassi(valor);
            
            Assert.Equal(valor.ToUpper(), chassi.Valor);
        }

        [Fact]
        public void ToString_DeveRetornarValorDoChassi()
        {            
            var valor = "9BWZZZ377VT004251";
            var chassi = new Chassi(valor);
            
            var resultado = chassi.ToString();
            
            Assert.Equal(valor, resultado);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComValorNuloOuVazio_DeveLancarChassiInvalidoException(string? valor)
        {            
            var exception = Assert.Throws<ChassiInvalidoException>(
                () => new Chassi(valor!));

            Assert.Equal("Chassi inválido.", exception.Message);
        }

        [Theory]
        [InlineData("1234567890123456")]     
        [InlineData("123456789012345678")]   
        [InlineData("123")]                  
        public void Construtor_ComQuantidadeDeCaracteresInvalida_DeveLancarChassiInvalidoException(string valor)
        {            
            var exception = Assert.Throws<ChassiInvalidoException>(
                () => new Chassi(valor));

            Assert.Equal("Chassi deve possuir 17 caracteres.", exception.Message);
        }
    }
}
