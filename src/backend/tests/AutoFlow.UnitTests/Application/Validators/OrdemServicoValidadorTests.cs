using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;

namespace AutoFlow.UnitTests.Application.Validators
{
    public class OrdemServicoValidadorTests
    {
        [Fact]
        public void Validar_CriacaoComClienteInvalido_DeveRetornarValidation()
        {
            var resultado = OrdemServicoValidador.Validar(
                new CriaOrdemServicoDto(0, 1, null));

            Assert.NotNull(resultado);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
            Assert.Equal("Cliente é obrigatório.", resultado.Error);
        }

        [Fact]
        public void Validar_DiagnosticoVazio_DeveRetornarValidation()
        {
            var resultado = OrdemServicoValidador.Validar(
                new RegistraDiagnosticoOrdemServicoDto(""));

            Assert.NotNull(resultado);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
            Assert.Equal("Diagnóstico é obrigatório.", resultado.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validar_ServicoComQuantidadeInvalida_DeveRetornarValidation(int quantidade)
        {
            var resultado = OrdemServicoValidador.Validar(
                new AdicionaServicoOrdemServicoDto(1, quantidade));

            Assert.NotNull(resultado);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_DadosValidos_DeveRetornarNull()
        {
            var resultado = OrdemServicoValidador.Validar(
                new CriaOrdemServicoDto(1, 2, "Sem avarias"));

            Assert.Null(resultado);
        }
    }
}
