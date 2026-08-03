using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;

namespace AutoFlow.UnitTests.Application.Validators
{
    public class ClienteValidadorTests
    {
        private static CriaClienteDto DtoValido() =>
            new("João da Silva", "12345678901", "11999999999", "joao@email.com");

        [Fact]
        public void Validar_ComDadosValidos_DeveRetornarNull()
        {
            var resultado = ClienteValidador.Validar(DtoValido());

            Assert.Null(resultado);
        }

        [Fact]
        public void Validar_ComNomeVazio_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Nome = "" };

            var resultado = ClienteValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.False(resultado!.IsSuccess);
            Assert.Equal("Nome é obrigatório.", resultado.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_ComDocumentoVazio_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Documento = "" };

            var resultado = ClienteValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Documento é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_ComTelefoneVazio_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Telefone = "" };

            var resultado = ClienteValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Telefone é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }

        [Fact]
        public void Validar_ComEmailVazio_DeveRetornarFailureDeValidacao()
        {
            var dto = DtoValido() with { Email = "" };

            var resultado = ClienteValidador.Validar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Email é obrigatório.", resultado!.Error);
            Assert.Equal(ErrorType.Validation, resultado.ErrorType);
        }
    }
}
