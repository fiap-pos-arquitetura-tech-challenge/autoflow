using AutoFlow.Application.DTOs;
using AutoFlow.Domain.Enums;
using AutoFlow.IntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AutoFlow.IntegrationTests.Api;

public class OrdemServicoEndpointsTests(AutoFlowApplicationFactory factory) : IClassFixture<AutoFlowApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static int _sequenciaVeiculo;

    private async Task AutenticarColaboradorAsync()
    {
        var token = await AuthHelper.LoginColaboradorAsync(
            _client,
            AutoFlowApplicationFactory.ColaboradorEmail,
            AutoFlowApplicationFactory.ColaboradorSenha);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task AutenticarClienteAsync(string email, string senha)
    {
        var token = await AuthHelper.LoginClienteAsync(_client, email, senha);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<OrdemServicoDto> CriarOrdemAsync()
    {
        await AutenticarColaboradorAsync();

        var sequencia = Interlocked.Increment(ref _sequenciaVeiculo);
        var letra = (char)('A' + ((sequencia - 1) % 26));
        var placa = $"OS{letra}1A{sequencia % 100:00}";
        var chassi = $"9BWZZZ377VT{sequencia:000000}";

        var veiculoDto = new CriaVeiculoDto(
            ClienteId: AutoFlowApplicationFactory.ClienteId,
            Marca: "Toyota",
            Modelo: "Corolla",
            AnoFabricacao: 2020,
            AnoModelo: 2021,
            Cor: "Preto",
            Tipo: TipoVeiculo.Carro,
            Combustivel: Combustivel.Gasolina,
            Placa: placa,
            Chassi: chassi,
            Quilometragem: 10000);

        var veiculoResponse = await _client.PostAsJsonAsync("/api/veiculos", veiculoDto);
        veiculoResponse.EnsureSuccessStatusCode();

        var veiculo = await veiculoResponse.Content.ReadFromJsonAsync<VeiculoDto>();

        var ordemResponse = await _client.PostAsJsonAsync(
            "/api/ordens-servico",
            new CriaOrdemServicoDto(
                AutoFlowApplicationFactory.ClienteId,
                veiculo!.Id,
                "Risco no para-choque"));

        ordemResponse.EnsureSuccessStatusCode();

        return (await ordemResponse.Content.ReadFromJsonAsync<OrdemServicoDto>())!;
    }

    private async Task<OrdemServicoDto> PrepararOrdemAguardandoAprovacaoAsync()
    {
        var ordem = await CriarOrdemAsync();
        await AutenticarColaboradorAsync();

        var iniciarDiagnostico = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/diagnostico/iniciar",
            null);
        iniciarDiagnostico.EnsureSuccessStatusCode();

        var registrarDiagnostico = await _client.PutAsJsonAsync(
            $"/api/ordens-servico/{ordem.Id}/diagnostico",
            new RegistraDiagnosticoOrdemServicoDto("Serviço necessário."));
        registrarDiagnostico.EnsureSuccessStatusCode();

        var sequencia = Interlocked.Increment(ref _sequenciaVeiculo);
        var servicoResponse = await _client.PostAsJsonAsync(
            "/api/servicos",
            new CriaServicoDto($"Serviço aprovação OS {sequencia}", 100m, 30));
        servicoResponse.EnsureSuccessStatusCode();
        var servico = (await servicoResponse.Content.ReadFromJsonAsync<ServicoDto>())!;

        var adicionarServico = await _client.PostAsJsonAsync(
            $"/api/ordens-servico/{ordem.Id}/servicos",
            new AdicionaServicoOrdemServicoDto(servico.Id, 1));
        adicionarServico.EnsureSuccessStatusCode();

        var gerarOrcamento = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento",
            null);
        gerarOrcamento.EnsureSuccessStatusCode();

        return (await gerarOrcamento.Content.ReadFromJsonAsync<OrdemServicoDto>())!;
    }

    [Fact]
    public async Task GetAndamento_ComClienteDaOrdem_DeveRetornar200()
    {
        var ordem = await CriarOrdemAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.ClienteEmail,
            AutoFlowApplicationFactory.ClienteSenha);

        var response = await _client.GetAsync($"/api/ordens-servico/{ordem.Id}/andamento");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<AndamentoOrdemServicoDto>();

        Assert.NotNull(resultado);
        Assert.Equal(ordem.Id, resultado!.Id);
        Assert.Equal(StatusOrdemServico.Recebida, resultado.Status);
    }

    [Fact]
    public async Task GetAndamento_ComOutroCliente_DeveRetornar404()
    {
        var ordem = await CriarOrdemAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.OutroClienteEmail,
            AutoFlowApplicationFactory.OutroClienteSenha);

        var response = await _client.GetAsync($"/api/ordens-servico/{ordem.Id}/andamento");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAndamento_ComColaborador_DeveRetornar200()
    {
        var ordem = await CriarOrdemAsync();

        await AutenticarColaboradorAsync();

        var response = await _client.GetAsync($"/api/ordens-servico/{ordem.Id}/andamento");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAndamento_SemToken_DeveRetornar401()
    {
        var ordem = await CriarOrdemAsync();
        var clienteAnonimo = factory.CreateClient();

        var response = await clienteAnonimo.GetAsync($"/api/ordens-servico/{ordem.Id}/andamento");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetOrdemCompleta_ComPerfilCliente_DeveRetornar403()
    {
        var ordem = await CriarOrdemAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.ClienteEmail,
            AutoFlowApplicationFactory.ClienteSenha);

        var response = await _client.GetAsync($"/api/ordens-servico/{ordem.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetOrcamento_ComClienteDaOrdem_DeveRetornar200ComItens()
    {
        var ordem = await PrepararOrdemAguardandoAprovacaoAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.ClienteEmail,
            AutoFlowApplicationFactory.ClienteSenha);

        var response = await _client.GetAsync($"/api/ordens-servico/{ordem.Id}/orcamento");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = (await response.Content.ReadFromJsonAsync<OrcamentoClienteDto>())!;
        Assert.Equal(ordem.Id, resultado.OrdemServicoId);
        Assert.Single(resultado.Servicos);
        Assert.Equal(100m, resultado.Orcamento.ValorTotal);
    }

    [Fact]
    public async Task GetOrcamento_ComOutroCliente_DeveRetornar404()
    {
        var ordem = await PrepararOrdemAguardandoAprovacaoAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.OutroClienteEmail,
            AutoFlowApplicationFactory.OutroClienteSenha);

        var response = await _client.GetAsync($"/api/ordens-servico/{ordem.Id}/orcamento");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AprovarOrcamento_ComClienteDaOrdem_DeveRetornar200()
    {
        var ordem = await PrepararOrdemAguardandoAprovacaoAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.ClienteEmail,
            AutoFlowApplicationFactory.ClienteSenha);

        var response = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento/aprovar",
            null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = (await response.Content.ReadFromJsonAsync<OrdemServicoDto>())!;
        Assert.Equal(StatusOrdemServico.EmExecucao, resultado.Status);
        Assert.Equal(StatusOrcamento.Aprovado, resultado.Orcamento!.Status);
    }

    [Fact]
    public async Task AprovarOrcamento_ComOutroCliente_DeveRetornar404()
    {
        var ordem = await PrepararOrdemAguardandoAprovacaoAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.OutroClienteEmail,
            AutoFlowApplicationFactory.OutroClienteSenha);

        var response = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento/aprovar",
            null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AprovarOrcamento_ComColaborador_DeveRetornar403()
    {
        var ordem = await PrepararOrdemAguardandoAprovacaoAsync();
        await AutenticarColaboradorAsync();

        var response = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento/aprovar",
            null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ReprovarOrcamento_ComClienteDaOrdem_DeveRetornar200()
    {
        var ordem = await PrepararOrdemAguardandoAprovacaoAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.ClienteEmail,
            AutoFlowApplicationFactory.ClienteSenha);

        var response = await _client.PostAsJsonAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento/reprovar",
            new ReprovaOrcamentoOrdemServicoDto("Valor acima do esperado."));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = (await response.Content.ReadFromJsonAsync<OrdemServicoDto>())!;
        Assert.Equal(StatusOrdemServico.EmDiagnostico, resultado.Status);
        Assert.Equal(StatusOrcamento.Reprovado, resultado.Orcamento!.Status);
    }

    [Fact]
    public async Task ReprovarOrcamento_ComOutroCliente_DeveRetornar404()
    {
        var ordem = await PrepararOrdemAguardandoAprovacaoAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.OutroClienteEmail,
            AutoFlowApplicationFactory.OutroClienteSenha);

        var response = await _client.PostAsJsonAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento/reprovar",
            new ReprovaOrcamentoOrdemServicoDto("Valor acima do esperado."));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Finalizar_ComServicoPendente_DeveRetornar409()
    {
        var ordem = await PrepararOrdemAguardandoAprovacaoAsync();

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.ClienteEmail,
            AutoFlowApplicationFactory.ClienteSenha);

        var aprovar = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento/aprovar",
            null);
        aprovar.EnsureSuccessStatusCode();

        await AutenticarColaboradorAsync();

        var response = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/finalizar",
            null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task FluxoCompleto_ComServicoEPeca_DeveEntregarOrdemEBaixarEstoque()
    {
        var ordem = await CriarOrdemAsync();
        await AutenticarColaboradorAsync();

        var iniciarDiagnostico = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/diagnostico/iniciar",
            null);
        Assert.Equal(HttpStatusCode.OK, iniciarDiagnostico.StatusCode);

        var registrarDiagnostico = await _client.PutAsJsonAsync(
            $"/api/ordens-servico/{ordem.Id}/diagnostico",
            new RegistraDiagnosticoOrdemServicoDto("Troca de óleo e filtro necessária."));
        Assert.Equal(HttpStatusCode.OK, registrarDiagnostico.StatusCode);

        var sequencia = Interlocked.Increment(ref _sequenciaVeiculo);

        var servicoResponse = await _client.PostAsJsonAsync(
            "/api/servicos",
            new CriaServicoDto($"Troca de óleo OS {sequencia}", 200m, 60));
        servicoResponse.EnsureSuccessStatusCode();
        var servico = (await servicoResponse.Content.ReadFromJsonAsync<ServicoDto>())!;

        var pecaResponse = await _client.PostAsJsonAsync(
            "/api/pecasInsumos",
            new CriaPecaInsumoDto($"Filtro OS {sequencia}", 50m));
        pecaResponse.EnsureSuccessStatusCode();
        var peca = (await pecaResponse.Content.ReadFromJsonAsync<PecaInsumoDto>())!;

        var estoqueResponse = await _client.GetAsync($"/api/estoques/peca/{peca.Id}");
        estoqueResponse.EnsureSuccessStatusCode();
        var estoque = (await estoqueResponse.Content.ReadFromJsonAsync<EstoqueDto>())!;
        Assert.Equal(0, estoque.Quantidade);

        var entradaEstoque = await _client.PostAsJsonAsync(
            $"/api/estoques/{estoque.Id}/entrada",
            new MovimentaEstoqueDto(10));
        entradaEstoque.EnsureSuccessStatusCode();

        var adicionarServico = await _client.PostAsJsonAsync(
            $"/api/ordens-servico/{ordem.Id}/servicos",
            new AdicionaServicoOrdemServicoDto(servico.Id, 1));
        adicionarServico.EnsureSuccessStatusCode();

        var adicionarPeca = await _client.PostAsJsonAsync(
            $"/api/ordens-servico/{ordem.Id}/pecas",
            new AdicionaPecaOrdemServicoDto(peca.Id, 2));
        adicionarPeca.EnsureSuccessStatusCode();

        var gerarOrcamento = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento",
            null);
        gerarOrcamento.EnsureSuccessStatusCode();

        var ordemComOrcamento = (await gerarOrcamento.Content.ReadFromJsonAsync<OrdemServicoDto>())!;
        Assert.NotNull(ordemComOrcamento.Orcamento);
        Assert.Equal(300m, ordemComOrcamento.Orcamento!.ValorTotal);
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, ordemComOrcamento.Status);

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.ClienteEmail,
            AutoFlowApplicationFactory.ClienteSenha);

        var aprovarOrcamento = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/orcamento/aprovar",
            null);
        aprovarOrcamento.EnsureSuccessStatusCode();

        var ordemEmExecucao = (await aprovarOrcamento.Content.ReadFromJsonAsync<OrdemServicoDto>())!;
        Assert.Equal(StatusOrdemServico.EmExecucao, ordemEmExecucao.Status);
        Assert.Equal(StatusOrcamento.Aprovado, ordemEmExecucao.Orcamento!.Status);

        await AutenticarColaboradorAsync();

        var estoqueAposAprovacaoResponse = await _client.GetAsync($"/api/estoques/peca/{peca.Id}");
        estoqueAposAprovacaoResponse.EnsureSuccessStatusCode();
        var estoqueAposAprovacao = (await estoqueAposAprovacaoResponse.Content.ReadFromJsonAsync<EstoqueDto>())!;
        Assert.Equal(8, estoqueAposAprovacao.Quantidade);

        var itemServico = Assert.Single(ordemEmExecucao.Servicos);

        var iniciarServico = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/servicos/{itemServico.Id}/execucao/iniciar",
            null);
        iniciarServico.EnsureSuccessStatusCode();

        var finalizarServico = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/servicos/{itemServico.Id}/execucao/finalizar",
            null);
        finalizarServico.EnsureSuccessStatusCode();

        var ordemComServicoFinalizado = (await finalizarServico.Content.ReadFromJsonAsync<OrdemServicoDto>())!;
        var servicoExecutado = Assert.Single(ordemComServicoFinalizado.Servicos);
        Assert.NotNull(servicoExecutado.ExecucaoIniciadaEm);
        Assert.NotNull(servicoExecutado.ExecucaoFinalizadaEm);

        var finalizarOrdem = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/finalizar",
            null);
        finalizarOrdem.EnsureSuccessStatusCode();

        var entregarOrdem = await _client.PostAsync(
            $"/api/ordens-servico/{ordem.Id}/entregar",
            null);
        entregarOrdem.EnsureSuccessStatusCode();

        var ordemEntregue = (await entregarOrdem.Content.ReadFromJsonAsync<OrdemServicoDto>())!;
        Assert.Equal(StatusOrdemServico.Entregue, ordemEntregue.Status);
        Assert.NotNull(ordemEntregue.FinalizadaEm);
        Assert.NotNull(ordemEntregue.EntregueEm);

        await AutenticarClienteAsync(
            AutoFlowApplicationFactory.ClienteEmail,
            AutoFlowApplicationFactory.ClienteSenha);

        var andamentoResponse = await _client.GetAsync(
            $"/api/ordens-servico/{ordem.Id}/andamento");
        andamentoResponse.EnsureSuccessStatusCode();

        var andamento = (await andamentoResponse.Content.ReadFromJsonAsync<AndamentoOrdemServicoDto>())!;
        Assert.Equal(StatusOrdemServico.Entregue, andamento.Status);
        Assert.NotNull(andamento.DiagnosticoIniciadoEm);
        Assert.NotNull(andamento.OrcamentoGeradoEm);
        Assert.NotNull(andamento.OrcamentoDecididoEm);
        Assert.NotNull(andamento.ExecucaoIniciadaEm);
        Assert.NotNull(andamento.FinalizadaEm);
        Assert.NotNull(andamento.EntregueEm);
    }

}
