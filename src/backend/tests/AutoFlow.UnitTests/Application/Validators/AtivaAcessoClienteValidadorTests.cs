using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;

namespace AutoFlow.UnitTests.Application.Validators
{
    public class AtivaAcessoClienteValidadorTests
    {
        private static AtivaAcessoClienteDto DtoValido() =>
            new("11144477735", "SenhaForte@123");

        [Fact]
        public void Validar_ComDadosValidos_DeveRetornarNull()
        {
            var resultado = AtivaAcessoClienteValidador.Validar(DtoValido());

            Assert.Null(resultado);
        }

        [Fact]
        public void Validar_ComDocumentoVazio_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Documento = "" };

            var resultado = AtivaAcessoClienteValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Documento é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_ComSenhaVazia_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Senha = "" };

            var resultado = AtivaAcessoClienteValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Senha é obrigatória.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }
    }
}
