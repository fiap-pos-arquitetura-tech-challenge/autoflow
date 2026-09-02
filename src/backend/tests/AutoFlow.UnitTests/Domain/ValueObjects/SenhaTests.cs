using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.UnitTests.Domain.ValueObjects
{
    public class SenhaTests
    {
        [Fact]
        public void Criar_ComSenhaValida_DeveGerarHashESalt()
        {
            var senha = Senha.Criar("SenhaForte@123");

            Assert.NotEmpty(senha.Hash);
            Assert.NotEmpty(senha.Salt);
        }

        [Fact]
        public void Criar_ComAMesmaSenhaDuasVezes_DeveGerarHashESaltDiferentes()
        {
            var senha1 = Senha.Criar("SenhaForte@123");
            var senha2 = Senha.Criar("SenhaForte@123");

            Assert.NotEqual(senha1.Salt, senha2.Salt);
            Assert.NotEqual(senha1.Hash, senha2.Hash);
        }

        [Fact]
        public void Verificar_ComSenhaCorreta_DeveRetornarTrue()
        {
            var senha = Senha.Criar("SenhaForte@123");

            Assert.True(senha.Verificar("SenhaForte@123"));
        }

        [Fact]
        public void Verificar_ComSenhaIncorreta_DeveRetornarFalse()
        {
            var senha = Senha.Criar("SenhaForte@123");

            Assert.False(senha.Verificar("OutraSenha@123"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Criar_ComSenhaNulaOuVazia_DeveLancarSenhaInvalidaException(string? senhaInvalida)
        {
            var exception = Assert.Throws<SenhaInvalidaException>(() => Senha.Criar(senhaInvalida!));

            Assert.Equal("Senha é obrigatória.", exception.Message);
        }

        [Fact]
        public void Criar_ComSenhaMenorQueOMinimo_DeveLancarSenhaInvalidaException()
        {
            var exception = Assert.Throws<SenhaInvalidaException>(() => Senha.Criar("abc123"));

            Assert.Equal("Senha deve ter no mínimo 8 caracteres.", exception.Message);
        }
    }
}
