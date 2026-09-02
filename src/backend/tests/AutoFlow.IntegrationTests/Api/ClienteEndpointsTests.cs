
using AutoFlow.Application.DTOs;
using AutoFlow.IntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AutoFlow.IntegrationTests.Api;

public class ClienteEndpointsTests(AutoFlowApplicationFactory factory) : IClassFixture<AutoFlowApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CriaClienteDto CriarClienteDto(string documento = "52998224725")
    {
        return new CriaClienteDto(
            Nome: "Maria Souza",
            Documento: documento,
            Telefone: "11988887777",
            Email: "maria.souza@email.com"
        );
    }

    private static AtualizaClienteDto CriarAtualizacaoDto(string documento = "98765432029")
    {
        return new AtualizaClienteDto(
            Nome: "Maria Souza Atualizada",
            Documento: documento,
            Telefone: "11977776666",
            Email: "maria.atualizada@email.com"
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

    private async Task<ClienteDto> CriarClienteAsync(string documento)
    {
        await AutenticarAsync();

        var response = await _client.PostAsJsonAsync("/api/clientes", CriarClienteDto(documento));

        response.EnsureSuccessStatusCode();

        var resultado = await response.Content.ReadFromJsonAsync<ClienteDto>();

        return resultado!;
    }

    [Fact]
    public async Task PostCliente_ComDadosValidos_DeveRetornar201()
    {
        await AutenticarAsync();

        var dto = CriarClienteDto("11122233396");

        var response = await _client.PostAsJsonAsync("/api/clientes", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<ClienteDto>();

        Assert.NotNull(resultado);

        Assert.True(resultado!.Id > 0);
        Assert.Equal(dto.Nome, resultado.Nome);
        Assert.Equal(dto.Documento, resultado.Documento);
        Assert.Equal(dto.Telefone, resultado.Telefone);
        Assert.Equal(dto.Email, resultado.Email);
    }

    [Fact]
    public async Task PostCliente_SemToken_DeveRetornar401()
    {
        var clienteAnonimo = factory.CreateClient();

        var response = await clienteAnonimo.PostAsJsonAsync("/api/clientes", CriarClienteDto("22233344405"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostCliente_ComDadosInvalidos_DeveRetornar400()
    {
        await AutenticarAsync();

        var dto = CriarClienteDto() with { Nome = "" };

        var response = await _client.PostAsJsonAsync("/api/clientes", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostCliente_ComDocumentoDuplicado_DeveRetornar409()
    {
        await AutenticarAsync();

        var dto = CriarClienteDto(AutoFlowApplicationFactory.ClienteDocumento);

        var response = await _client.PostAsJsonAsync("/api/clientes", dto);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetClientePorId_ComIdExistente_DeveRetornar200()
    {
        var criado = await CriarClienteAsync("33344455508");

        var response = await _client.GetAsync($"/api/clientes/{criado.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<ClienteDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
        Assert.Equal(criado.Documento, resultado.Documento);
    }

    [Fact]
    public async Task GetClientePorId_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.GetAsync("/api/clientes/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTodosClientes_DeveRetornar200EConterClienteCriado()
    {
        var criado = await CriarClienteAsync("44455566619");

        var response = await _client.GetAsync("/api/clientes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<List<ClienteDto>>();

        Assert.NotNull(resultado);
        Assert.Contains(resultado!, c => c.Id == criado.Id);
    }

    [Fact]
    public async Task PutCliente_ComDadosValidos_DeveRetornar200()
    {
        var criado = await CriarClienteAsync("55566677720");

        var dto = CriarAtualizacaoDto("66677788830");

        var response = await _client.PutAsJsonAsync($"/api/clientes/{criado.Id}", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<ClienteDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
        Assert.Equal(dto.Nome, resultado.Nome);
        Assert.Equal(dto.Documento, resultado.Documento);
        Assert.Equal(dto.Telefone, resultado.Telefone);
        Assert.Equal(dto.Email, resultado.Email);
    }

    [Fact]
    public async Task PutCliente_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.PutAsJsonAsync(
            "/api/clientes/999999",
            CriarAtualizacaoDto("77788899941"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCliente_ComIdExistente_DeveRetornar204()
    {
        var criado = await CriarClienteAsync("88899900078");

        var response = await _client.DeleteAsync($"/api/clientes/{criado.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/clientes/{criado.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteCliente_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.DeleteAsync("/api/clientes/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
