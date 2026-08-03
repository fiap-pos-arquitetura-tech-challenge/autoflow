using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.UnitTests.Application.Services
{
    public class ResultTests
    {
        [Fact]
        public void Success_DeveCriarResultComIsSuccessVerdadeiro()
        {
            var result = Result.Success();

            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);
            Assert.Equal(ErrorType.None, result.ErrorType);
        }

        [Fact]
        public void Failure_DeveCriarResultComErroEIsSuccessFalso()
        {
            var result = Result.Failure("erro", ErrorType.Validation);

            Assert.False(result.IsSuccess);
            Assert.Equal("erro", result.Error);
            Assert.Equal(ErrorType.Validation, result.ErrorType);
        }

        [Fact]
        public void SuccessGenerico_DeveCriarResultComValorEIsSuccessVerdadeiro()
        {
            var result = Result<string>.Success("valor");

            Assert.True(result.IsSuccess);
            Assert.Equal("valor", result.Value);
            Assert.Null(result.Error);
            Assert.Equal(ErrorType.None, result.ErrorType);
        }

        [Fact]
        public void FailureGenerico_DeveCriarResultComErroEValorPadrao()
        {
            var result = Result<string>.Failure("erro", ErrorType.NotFound);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("erro", result.Error);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
        }
    }
}
