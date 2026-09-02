using AutoFlow.Api.Extensions;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using Microsoft.AspNetCore.Http;
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
        public void ToCreatedHttpResult_ComFailureDeValidacao_DeveRetornarProblemComBadRequest()
        {
            var result = Result<string>.Failure("erro", ErrorType.Validation);

            var httpResult = result.ToCreatedHttpResult(v => $"/recursos/{v}");

            var problem = Assert.IsType<ProblemHttpResult>(httpResult);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.ProblemDetails.Status);
            Assert.Equal("erro", problem.ProblemDetails.Detail);
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
        public void ToHttpResultGenerico_ComFailureNotFound_DeveRetornarProblemComNotFound()
        {
            var result = Result<string>.Failure("não encontrado", ErrorType.NotFound);

            var httpResult = result.ToHttpResult();

            var problem = Assert.IsType<ProblemHttpResult>(httpResult);
            Assert.Equal(StatusCodes.Status404NotFound, problem.ProblemDetails.Status);
            Assert.Equal("não encontrado", problem.ProblemDetails.Detail);
        }

        [Fact]
        public void ToHttpResultGenerico_ComFailureConflict_DeveRetornarProblemComConflict()
        {
            var result = Result<string>.Failure("conflito", ErrorType.Conflict);

            var httpResult = result.ToHttpResult();

            var problem = Assert.IsType<ProblemHttpResult>(httpResult);
            Assert.Equal(StatusCodes.Status409Conflict, problem.ProblemDetails.Status);
            Assert.Equal("conflito", problem.ProblemDetails.Detail);
        }

        [Fact]
        public void ToHttpResultGenerico_ComFailureUnauthorized_DeveRetornarProblemComUnauthorized()
        {
            var result = Result<string>.Failure("credenciais inválidas", ErrorType.Unauthorized);

            var httpResult = result.ToHttpResult();

            var problem = Assert.IsType<ProblemHttpResult>(httpResult);
            Assert.Equal(StatusCodes.Status401Unauthorized, problem.ProblemDetails.Status);
            Assert.Equal("credenciais inválidas", problem.ProblemDetails.Detail);
        }

        [Fact]
        public void ToHttpResultGenerico_ComFailureSemTipoMapeado_DeveRetornarProblemComInternalServerError()
        {
            var result = Result<string>.Failure("erro inesperado", ErrorType.None);

            var httpResult = result.ToHttpResult();

            var problem = Assert.IsType<ProblemHttpResult>(httpResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, problem.ProblemDetails.Status);
        }

        [Fact]
        public void ToHttpResultNaoGenerico_ComSucesso_DeveRetornarNoContent()
        {
            var result = Result.Success();

            var httpResult = result.ToHttpResult();

            Assert.IsType<NoContent>(httpResult);
        }

        [Fact]
        public void ToHttpResultNaoGenerico_ComFailureNotFound_DeveRetornarProblemComNotFound()
        {
            var result = Result.Failure("não encontrado", ErrorType.NotFound);

            var httpResult = result.ToHttpResult();

            var problem = Assert.IsType<ProblemHttpResult>(httpResult);
            Assert.Equal(StatusCodes.Status404NotFound, problem.ProblemDetails.Status);
            Assert.Equal("não encontrado", problem.ProblemDetails.Detail);
        }
    }
}
