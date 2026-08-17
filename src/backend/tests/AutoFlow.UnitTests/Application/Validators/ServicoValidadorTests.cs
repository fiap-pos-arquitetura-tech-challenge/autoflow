using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;

namespace AutoFlow.UnitTests.Application.Validators
{
    public class ServicoValidadorTests
    {
        private static CriaServicoDto DtoValido() =>
            new("Troca de óleo", 150m, 60);

        [Fact]
        public void Validar_ComDadosValidos_DeveRetornarNull()
        {
            var resultado = ServicoValidador.Validar(DtoValido());

            Assert.Null(resultado);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validar_ComNomeVazio_DeveRetornarFailureDeValidacao(
            string? nome)
        {
            var dto = DtoValido() with
            {
                Nome = nome!
            };

            var resultado = ServicoValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.False(resultado!.IsSuccess);
            Assert.Equal("Nome é obrigatório.", resultado.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456")]
        [InlineData("999")]
        public void Validar_ComNomeSomenteNumeros_DeveRetornarFailureDeValidacao(
            string nome)
        {
            var dto = DtoValido() with
            {
                Nome = nome
            };

            var resultado = ServicoValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.False(resultado!.IsSuccess);
            Assert.Equal("Nome do serviço inválido.", resultado.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Validar_ComPrecoMenorOuIgualAZero_DeveRetornarFailureDeValidacao(
            decimal preco)
        {
            var dto = DtoValido() with
            {
                Preco = preco
            };

            var resultado = ServicoValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.False(resultado!.IsSuccess);
            Assert.Equal("Preço deve ser maior que zero.", resultado.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        public void Validar_ComTempoMedioMenorOuIgualAUm_DeveRetornarFailureDeValidacao(
            int tempoMedio)
        {
            var dto = DtoValido() with
            {
                TempoMedio = tempoMedio
            };

            var resultado = ServicoValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.False(resultado!.IsSuccess);
            Assert.Equal("Tempo médio deve ser maior que 1.", resultado.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }
    }
}