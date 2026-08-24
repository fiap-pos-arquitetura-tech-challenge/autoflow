
using AutoFlow.Application.DTOs;
using AutoFlow.IntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AutoFlow.IntegrationTests.Api;

public class PecaInsumoEndpointsTests(AutoFlowApplicationFactory factory) : IClassFixture<AutoFlowApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CriaPecaInsumoDto CriarPecaInsumoDto(string nome)
    {
        return new CriaPecaInsumoDto(
            Nome: nome,
            Valor: 89.90m
        );
    }

    private static AtualizaPecaInsumoDto CriarAtualizacaoDto(string nome)
    {
        return new AtualizaPecaInsumoDto(
            Nome: nome,
            Valor: 129.90m
        );
    }

    private async Task AutenticarAsync()
    {
        var token = await AuthHelper.LoginColaboradorAsync(
            _client,
            AutoFlowApplicationFactory.ColaboradorEmail,
            AutoFlowApplicationFactory.ColaboradorSenha);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<PecaInsumoDto> CriarPecaInsumoAsync(string nome)
    {
        await AutenticarAsync();

        var response = await _client.PostAsJsonAsync("/api/pecasInsumos", CriarPecaInsumoDto(nome));

        response.EnsureSuccessStatusCode();

        var resultado = await response.Content.ReadFromJsonAsync<PecaInsumoDto>();

        return resultado!;
    }

    [Fact]
    public async Task PostPecaInsumo_ComDadosValidos_DeveRetornar201()
    {
        await AutenticarAsync();

        var dto = CriarPecaInsumoDto("Filtro de óleo");

        var response = await _client.PostAsJsonAsync("/api/pecasInsumos", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<PecaInsumoDto>();

        Assert.NotNull(resultado);

        Assert.True(resultado!.Id > 0);
        Assert.Equal(dto.Nome, resultado.Nome);
        Assert.Equal(dto.Valor, resultado.Valor);
    }

    [Fact]
    public async Task PostPecaInsumo_SemToken_DeveRetornar401()
    {
        var clienteAnonimo = factory.CreateClient();

        var response = await clienteAnonimo.PostAsJsonAsync("/api/pecasInsumos", CriarPecaInsumoDto("Peça sem token"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostPecaInsumo_ComDadosInvalidos_DeveRetornar400()
    {
        await AutenticarAsync();

        var dto = CriarPecaInsumoDto("");

        var response = await _client.PostAsJsonAsync("/api/pecasInsumos", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPecaInsumoPorId_ComIdExistente_DeveRetornar200()
    {
        var criado = await CriarPecaInsumoAsync("Pastilha de freio dianteira");

        var response = await _client.GetAsync($"/api/pecasInsumos/{criado.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<PecaInsumoDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
        Assert.Equal(criado.Nome, resultado.Nome);
    }

    [Fact]
    public async Task GetPecaInsumoPorId_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.GetAsync("/api/pecasInsumos/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTodosPecaInsumo_DeveRetornar200EConterPecaCriada()
    {
        var criado = await CriarPecaInsumoAsync("Correia dentada");

        var response = await _client.GetAsync("/api/pecasInsumos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<List<PecaInsumoDto>>();

        Assert.NotNull(resultado);
        Assert.Contains(resultado!, p => p.Id == criado.Id);
    }

    [Fact]
    public async Task PutPecaInsumo_ComDadosValidos_DeveRetornar200()
    {
        var criado = await CriarPecaInsumoAsync("Vela de ignição");

        var dto = CriarAtualizacaoDto("Vela de ignição iridium");

        var response = await _client.PutAsJsonAsync($"/api/pecasInsumos/{criado.Id}", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<PecaInsumoDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
        Assert.Equal(dto.Nome, resultado.Nome);
        Assert.Equal(dto.Valor, resultado.Valor);
    }

    [Fact]
    public async Task PutPecaInsumo_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.PutAsJsonAsync(
            "/api/pecasInsumos/999999",
            CriarAtualizacaoDto("Peça inexistente"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutPecaInsumo_ComDadosInvalidos_DeveRetornar400()
    {
        var criado = await CriarPecaInsumoAsync("Amortecedor traseiro");

        var dto = CriarAtualizacaoDto("");

        var response = await _client.PutAsJsonAsync($"/api/pecasInsumos/{criado.Id}", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeletePecaInsumo_ComIdExistente_DeveRetornar204()
    {
        var criado = await CriarPecaInsumoAsync("Bateria automotiva");

        var response = await _client.DeleteAsync($"/api/pecasInsumos/{criado.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/pecasInsumos/{criado.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeletePecaInsumo_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.DeleteAsync("/api/pecasInsumos/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
