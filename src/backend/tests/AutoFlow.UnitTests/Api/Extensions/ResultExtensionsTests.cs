using AutoFlow.Api.Extensions;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AutoFlow.UnitTests.Api.Extensions
{
    public class ResultExtensionsTests
    {
        [Fact]
        public void ToCreatedHttpResult_ComSucesso_DeveRetornarCreatedComLocationEValor()
        {
            var result = Result<string>.Success("valor");

            var httpResult = result.ToCreatedHttpResult(v => $"/recursos/{v}");

            var created = Assert.IsType<Created<string>>(httpResult);
            Assert.Equal("/recursos/valor", created.Location);
            Assert.Equal("valor", created.Value);
        }

        [Fact]
        public void ToCreatedHttpResult_ComFailureDeValidacao_DeveRetornarBadRequest()
        {
            var result = Result<string>.Failure("erro", ErrorType.Validation);

            var httpResult = result.ToCreatedHttpResult(v => $"/recursos/{v}");

            var badRequest = Assert.IsType<BadRequest<string>>(httpResult);
            Assert.Equal("erro", badRequest.Value);
        }

        [Fact]
        public void ToHttpResultGenerico_ComSucesso_DeveRetornarOkComValor()
        {
            var result = Result<string>.Success("valor");

            var httpResult = result.ToHttpResult();

            var ok = Assert.IsType<Ok<string>>(httpResult);
            Assert.Equal("valor", ok.Value);
        }

        [Fact]
        public void ToHttpResultGenerico_ComFailureNotFound_DeveRetornarNotFound()
        {
            var result = Result<string>.Failure("não encontrado", ErrorType.NotFound);

            var httpResult = result.ToHttpResult();

            var notFound = Assert.IsType<NotFound<string>>(httpResult);
            Assert.Equal("não encontrado", notFound.Value);
        }

        [Fact]
        public void ToHttpResultGenerico_ComFailureConflict_DeveRetornarConflict()
        {
            var result = Result<string>.Failure("conflito", ErrorType.Conflict);

            var httpResult = result.ToHttpResult();

            var conflict = Assert.IsType<Conflict<string>>(httpResult);
            Assert.Equal("conflito", conflict.Value);
        }

        [Fact]
        public void ToHttpResultGenerico_ComFailureSemTipoMapeado_DeveRetornarProblem()
        {
            var result = Result<string>.Failure("erro inesperado", ErrorType.None);

            var httpResult = result.ToHttpResult();

            Assert.IsType<ProblemHttpResult>(httpResult);
        }

        [Fact]
        public void ToHttpResultNaoGenerico_ComSucesso_DeveRetornarNoContent()
        {
            var result = Result.Success();

            var httpResult = result.ToHttpResult();

            Assert.IsType<NoContent>(httpResult);
        }

        [Fact]
        public void ToHttpResultNaoGenerico_ComFailureNotFound_DeveRetornarNotFound()
        {
            var result = Result.Failure("não encontrado", ErrorType.NotFound);

            var httpResult = result.ToHttpResult();

            var notFound = Assert.IsType<NotFound<string>>(httpResult);
            Assert.Equal("não encontrado", notFound.Value);
        }
    }
}
