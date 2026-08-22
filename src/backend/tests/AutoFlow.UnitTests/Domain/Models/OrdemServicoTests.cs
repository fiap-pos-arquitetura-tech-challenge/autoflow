using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;

namespace AutoFlow.UnitTests.Domain.Models
{
    public class OrdemServicoTests
    {
        private static OrdemServico CriarOrdemServicoEmDiagnostico()
        {
            var ordemServico = new OrdemServico(1, 1, "Risco no para-choque traseiro");
            ordemServico.IniciarDiagnostico();
            ordemServico.RegistrarDiagnostico("Necessária troca de óleo e filtro.");
            return ordemServico;
        }

        private static OrdemServico CriarOrdemServicoAguardandoAprovacao()
        {
            var ordemServico = CriarOrdemServicoEmDiagnostico();
            ordemServico.AdicionarServico(1, "Troca de óleo", 1, 150m, 60);
            ordemServico.AdicionarPeca(1, "Filtro de óleo", 1, 50m);
            ordemServico.GerarOrcamento();
            return ordemServico;
        }

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarOrdemComoRecebida()
        {
            var ordemServico = new OrdemServico(1, 2, "Risco na porta direita");

            Assert.Equal(1, ordemServico.ClienteId);
            Assert.Equal(2, ordemServico.VeiculoId);
            Assert.Equal(StatusOrdemServico.Recebida, ordemServico.Status);
            Assert.Equal("Risco na porta direita", ordemServico.AvariasObservadas);
            Assert.NotEqual(default, ordemServico.DataAbertura);
            Assert.Empty(ordemServico.Servicos);
            Assert.Empty(ordemServico.Pecas);
            Assert.Null(ordemServico.Orcamento);
        }

        [Theory]
        [InlineData(0, 1, "Cliente é obrigatório.")]
        [InlineData(-1, 1, "Cliente é obrigatório.")]
        [InlineData(1, 0, "Veículo é obrigatório.")]
        [InlineData(1, -1, "Veículo é obrigatório.")]
        public void Construtor_ComIdsInvalidos_DeveLancarOrdemServicoInvalidaException(
            int clienteId,
            int veiculoId,
            string mensagemEsperada)
        {
            var exception = Assert.Throws<OrdemServicoInvalidaException>(() =>
                new OrdemServico(clienteId, veiculoId));

            Assert.Equal(mensagemEsperada, exception.Message);
        }

        [Fact]
        public void IniciarDiagnostico_OrdemRecebida_DeveAtualizarStatusEData()
        {
            var ordemServico = new OrdemServico(1, 1);

            ordemServico.IniciarDiagnostico();

            Assert.Equal(StatusOrdemServico.EmDiagnostico, ordemServico.Status);
            Assert.NotNull(ordemServico.DiagnosticoIniciadoEm);
        }

        [Fact]
        public void AdicionarServico_EmDiagnostico_DeveAdicionarItemComSnapshot()
        {
            var ordemServico = CriarOrdemServicoEmDiagnostico();

            ordemServico.AdicionarServico(1, "Troca de óleo", 2, 150m, 60);

            var item = Assert.Single(ordemServico.Servicos);
            Assert.Equal(1, item.ServicoId);
            Assert.Equal("Troca de óleo", item.Descricao);
            Assert.Equal(2, item.Quantidade);
            Assert.Equal(150m, item.ValorUnitario);
            Assert.Equal(60, item.TempoPrevisto);
            Assert.Equal(300m, item.Subtotal);
        }

        [Fact]
        public void AdicionarPeca_EmDiagnostico_DeveAdicionarItemComSnapshot()
        {
            var ordemServico = CriarOrdemServicoEmDiagnostico();

            ordemServico.AdicionarPeca(1, "Filtro de óleo", 2, 50m);

            var item = Assert.Single(ordemServico.Pecas);
            Assert.Equal(1, item.PecaId);
            Assert.Equal("Filtro de óleo", item.Descricao);
            Assert.Equal(2, item.Quantidade);
            Assert.Equal(50m, item.ValorUnitario);
            Assert.Equal(100m, item.Subtotal);
        }

        [Fact]
        public void GerarOrcamento_ComItensValidos_DeveCalcularTotalEAguardarAprovacao()
        {
            var ordemServico = CriarOrdemServicoEmDiagnostico();
            ordemServico.AdicionarServico(1, "Troca de óleo", 2, 150m, 60);
            ordemServico.AdicionarPeca(1, "Filtro de óleo", 2, 50m);

            ordemServico.GerarOrcamento();

            Assert.Equal(StatusOrdemServico.AguardandoAprovacao, ordemServico.Status);
            Assert.NotNull(ordemServico.Orcamento);
            Assert.Equal(300m, ordemServico.Orcamento.ValorServicos);
            Assert.Equal(100m, ordemServico.Orcamento.ValorPecas);
            Assert.Equal(400m, ordemServico.Orcamento.ValorTotal);
            Assert.NotNull(ordemServico.OrcamentoGeradoEm);
        }

        [Fact]
        public void GerarOrcamento_SemDiagnostico_DeveLancarOrdemServicoInvalidaException()
        {
            var ordemServico = new OrdemServico(1, 1);
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(1, "Troca de óleo", 1, 150m, 60);

            var exception = Assert.Throws<OrdemServicoInvalidaException>(() =>
                ordemServico.GerarOrcamento());

            Assert.Equal(
                "O diagnóstico deve ser registrado antes de gerar o orçamento.",
                exception.Message);
        }

        [Fact]
        public void AprovarOrcamento_DeveIniciarExecucao()
        {
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();

            ordemServico.AprovarOrcamento();

            Assert.Equal(StatusOrdemServico.EmExecucao, ordemServico.Status);
            Assert.Equal(StatusOrcamento.Aprovado, ordemServico.Orcamento!.Status);
            Assert.NotNull(ordemServico.OrcamentoDecididoEm);
            Assert.NotNull(ordemServico.ExecucaoIniciadaEm);
        }

        [Fact]
        public void ReprovarOrcamento_ComJustificativa_DeveRetornarParaDiagnostico()
        {
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();

            ordemServico.ReprovarOrcamento("Valor acima do esperado.");

            Assert.Equal(StatusOrdemServico.EmDiagnostico, ordemServico.Status);
            Assert.Equal(StatusOrcamento.Reprovado, ordemServico.Orcamento!.Status);
            Assert.Equal("Valor acima do esperado.", ordemServico.Orcamento.JustificativaReprovacao);
            Assert.NotNull(ordemServico.OrcamentoDecididoEm);
        }

        [Fact]
        public void FluxoCompleto_ComOrcamentoAprovado_DeveFinalizarComoEntregue()
        {
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();

            ordemServico.AprovarOrcamento();
            ordemServico.Finalizar();
            ordemServico.Entregar();

            Assert.Equal(StatusOrdemServico.Entregue, ordemServico.Status);
            Assert.NotNull(ordemServico.FinalizadaEm);
            Assert.NotNull(ordemServico.EntregueEm);
        }

        [Fact]
        public void AdicionarServico_ForaDoDiagnostico_DeveLancarOrdemServicoInvalidaException()
        {
            var ordemServico = new OrdemServico(1, 1);

            Assert.Throws<OrdemServicoInvalidaException>(() =>
                ordemServico.AdicionarServico(1, "Troca de óleo", 1, 150m, 60));
        }

        [Fact]
        public void Entregar_OrdemNaoFinalizada_DeveLancarOrdemServicoInvalidaException()
        {
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();
            ordemServico.AprovarOrcamento();

            Assert.Throws<OrdemServicoInvalidaException>(() =>
                ordemServico.Entregar());
        }

        [Fact]
        public void GerarNovoOrcamento_AposReprovacao_DeveLimparDataDaDecisaoAnterior()
        {
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();
            ordemServico.ReprovarOrcamento("Valor acima do esperado.");

            Assert.NotNull(ordemServico.OrcamentoDecididoEm);

            ordemServico.GerarOrcamento();

            Assert.Equal(StatusOrdemServico.AguardandoAprovacao, ordemServico.Status);
            Assert.Equal(StatusOrcamento.Pendente, ordemServico.Orcamento!.Status);
            Assert.Null(ordemServico.OrcamentoDecididoEm);
        }

        [Fact]
        public void IniciarEFinalizarExecucaoServico_EmExecucao_DeveRegistrarDatasNoItem()
        {
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();
            var item = Assert.Single(ordemServico.Servicos);
            item.Id = 25;
            ordemServico.AprovarOrcamento();

            ordemServico.IniciarExecucaoServico(25);
            ordemServico.FinalizarExecucaoServico(25);

            Assert.NotNull(item.ExecucaoIniciadaEm);
            Assert.NotNull(item.ExecucaoFinalizadaEm);
        }
    }
}
