using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;

namespace AutoFlow.UnitTests.Application.Validators
{
    public class UsuarioValidadorTests
    {
        private static CriaColaboradorDto DtoValido() =>
            new("João da Silva", "joao@email.com", "SenhaForte@123");

        [Fact]
        public void Validar_ComDadosValidos_DeveRetornarNull()
        {
            var resultado = UsuarioValidador.Validar(DtoValido());

            Assert.Null(resultado);
        }

        [Fact]
        public void Validar_ComNomeVazio_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Nome = "" };

            var resultado = UsuarioValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Nome é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_ComEmailVazio_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Email = "" };

            var resultado = UsuarioValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Email é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_ComSenhaVazia_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Senha = "" };

            var resultado = UsuarioValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Senha é obrigatória.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }
    }
}
