
using AutoFlow.Application.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace AutoFlow.IntegrationTests.Api;

public class EstoqueEndpointsTests(AutoFlowApplicationFactory factory) : IClassFixture<AutoFlowApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<EstoqueDto> CriarPecaInsumoComEstoqueAsync(string nome)
    {
        var pecaResponse = await _client.PostAsJsonAsync(
            "/api/pecasInsumos",
            new CriaPecaInsumoDto(nome, 50m));

        pecaResponse.EnsureSuccessStatusCode();

        var peca = await pecaResponse.Content.ReadFromJsonAsync<PecaInsumoDto>();

        var estoqueResponse = await _client.GetAsync($"/api/estoques/peca/{peca!.Id}");

        estoqueResponse.EnsureSuccessStatusCode();

        var estoque = await estoqueResponse.Content.ReadFromJsonAsync<EstoqueDto>();

        return estoque!;
    }

    [Fact]
    public async Task GetEstoquePorId_ComIdExistente_DeveRetornar200()
    {
        var estoque = await CriarPecaInsumoComEstoqueAsync("Óleo de motor 5W30");

        var response = await _client.GetAsync($"/api/estoques/{estoque.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<EstoqueDto>();

        Assert.NotNull(resultado);
        Assert.Equal(estoque.Id, resultado!.Id);
        Assert.Equal(0, resultado.Quantidade);
    }

    [Fact]
    public async Task GetEstoquePorId_ComIdInexistente_DeveRetornar404()
    {
        var response = await _client.GetAsync("/api/estoques/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetEstoquePorPecaInsumo_ComIdExistente_DeveRetornar200()
    {
        var pecaResponse = await _client.PostAsJsonAsync(
            "/api/pecasInsumos",
            new CriaPecaInsumoDto("Filtro de ar", 35m));

        pecaResponse.EnsureSuccessStatusCode();

        var peca = await pecaResponse.Content.ReadFromJsonAsync<PecaInsumoDto>();

        var response = await _client.GetAsync($"/api/estoques/peca/{peca!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<EstoqueDto>();

        Assert.NotNull(resultado);
        Assert.Equal(0, resultado!.Quantidade);
    }

    [Fact]
    public async Task GetEstoquePorPecaInsumo_ComIdInexistente_DeveRetornar404()
    {
        var response = await _client.GetAsync("/api/estoques/peca/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostEntrada_ComQuantidadeValida_DeveRetornar200EAumentarEstoque()
    {
        var estoque = await CriarPecaInsumoComEstoqueAsync("Pneu aro 15");

        var response = await _client.PostAsJsonAsync(
            $"/api/estoques/{estoque.Id}/entrada",
            new MovimentaEstoqueDto(10));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<EstoqueDto>();

        Assert.NotNull(resultado);
        Assert.Equal(10, resultado!.Quantidade);
    }

    [Fact]
    public async Task PostEntrada_ComQuantidadeInvalida_DeveRetornar400()
    {
        var estoque = await CriarPecaInsumoComEstoqueAsync("Amortecedor dianteiro");

        var response = await _client.PostAsJsonAsync(
            $"/api/estoques/{estoque.Id}/entrada",
            new MovimentaEstoqueDto(0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostEntrada_ComIdInexistente_DeveRetornar404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/estoques/999999/entrada",
            new MovimentaEstoqueDto(10));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostSaida_ComQuantidadeValida_DeveRetornar200EDiminuirEstoque()
    {
        var estoque = await CriarPecaInsumoComEstoqueAsync("Disco de freio");

        await _client.PostAsJsonAsync(
            $"/api/estoques/{estoque.Id}/entrada",
            new MovimentaEstoqueDto(20));

        var response = await _client.PostAsJsonAsync(
            $"/api/estoques/{estoque.Id}/saida",
            new MovimentaEstoqueDto(8));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<EstoqueDto>();

        Assert.NotNull(resultado);
        Assert.Equal(12, resultado!.Quantidade);
    }

    [Fact]
    public async Task PostSaida_ComEstoqueInsuficiente_DeveRetornar400()
    {
        var estoque = await CriarPecaInsumoComEstoqueAsync("Kit embreagem");

        var response = await _client.PostAsJsonAsync(
            $"/api/estoques/{estoque.Id}/saida",
            new MovimentaEstoqueDto(5));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostSaida_ComIdInexistente_DeveRetornar404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/estoques/999999/saida",
            new MovimentaEstoqueDto(1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutAjustar_ComQuantidadeValida_DeveRetornar200()
    {
        var estoque = await CriarPecaInsumoComEstoqueAsync("Bomba de combustível");

        var response = await _client.PutAsJsonAsync(
            $"/api/estoques/{estoque.Id}/ajustar",
            new MovimentaEstoqueDto(42));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<EstoqueDto>();

        Assert.NotNull(resultado);
        Assert.Equal(42, resultado!.Quantidade);
    }

    [Fact]
    public async Task PutAjustar_ComQuantidadeNegativa_DeveRetornar400()
    {
        var estoque = await CriarPecaInsumoComEstoqueAsync("Radiador");

        var response = await _client.PutAsJsonAsync(
            $"/api/estoques/{estoque.Id}/ajustar",
            new MovimentaEstoqueDto(-1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutAjustar_ComIdInexistente_DeveRetornar404()
    {
        var response = await _client.PutAsJsonAsync(
            "/api/estoques/999999/ajustar",
            new MovimentaEstoqueDto(10));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
