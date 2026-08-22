
using AutoFlow.Application.DTOs;
using AutoFlow.Domain.Enums;
using AutoFlow.IntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AutoFlow.IntegrationTests.Api;

public class VeiculoEndpointsTests(AutoFlowApplicationFactory factory) : IClassFixture<AutoFlowApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CriaVeiculoDto CriarVeiculoDto(string placa = "ABC1D23", string chassi = "9BWZZZ377VT004251")
    {
        return new CriaVeiculoDto(
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
            Quilometragem: 10000
        );
    }

    private static AtualizaVeiculoDto CriarAtualizacaoDto(string placa = "XYZ9A99", string chassi = "9BWZZZ377VT004252")
    {
        return new AtualizaVeiculoDto(
            ClienteId: AutoFlowApplicationFactory.ClienteId,
            Marca: "Honda",
            Modelo: "Civic",
            AnoFabricacao: 2022,
            AnoModelo: 2023,
            Cor: "Branco",
            Tipo: TipoVeiculo.Carro,
            Combustivel: Combustivel.Etanol,
            Placa: placa,
            Chassi: chassi,
            Quilometragem: 20000
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

    private async Task<VeiculoDto> CriarVeiculoAsync(string placa, string chassi)
    {
        await AutenticarAsync();

        var response = await _client.PostAsJsonAsync("/api/veiculos", CriarVeiculoDto(placa, chassi));

        response.EnsureSuccessStatusCode();

        var resultado = await response.Content.ReadFromJsonAsync<VeiculoDto>();

        return resultado!;
    }

    [Fact]
    public async Task PostVeiculo_ComDadosValidos_DeveRetornar201()
    {
        await AutenticarAsync();

        var dto = CriarVeiculoDto("ABC1D23", "9BWZZZ377VT004251");

        var response = await _client.PostAsJsonAsync("/api/veiculos", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<VeiculoDto>();

        Assert.NotNull(resultado);

        Assert.True(resultado!.Id > 0);
        Assert.Equal(dto.ClienteId, resultado.ClienteID);
        Assert.Equal(dto.Marca, resultado.Marca);
        Assert.Equal(dto.Modelo, resultado.Modelo);
        Assert.Equal(dto.AnoFabricacao, resultado.AnoFabricacao);
        Assert.Equal(dto.AnoModelo, resultado.AnoModelo);
        Assert.Equal(dto.Cor, resultado.Cor);
        Assert.Equal(dto.Tipo, resultado.Tipo);
        Assert.Equal(dto.Combustivel, resultado.Combustivel);
        Assert.Equal(dto.Placa, resultado.Placa);
        Assert.Equal(dto.Chassi, resultado.Chassi);
        Assert.Equal(dto.Quilometragem, resultado.Quilometragem);
    }

    [Fact]
    public async Task PostVeiculo_SemToken_DeveRetornar401()
    {
        var clienteAnonimo = factory.CreateClient();

        var response = await clienteAnonimo.PostAsJsonAsync("/api/veiculos", CriarVeiculoDto("AAA1A11", "9BWZZZ377VT004261"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostVeiculo_ComDadosInvalidos_DeveRetornar400()
    {
        await AutenticarAsync();

        var dto = CriarVeiculoDto() with { Marca = "" };

        var response = await _client.PostAsJsonAsync("/api/veiculos", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetVeiculoPorId_ComIdExistente_DeveRetornar200()
    {
        var criado = await CriarVeiculoAsync("BBB2B22", "9BWZZZ377VT004262");

        var response = await _client.GetAsync($"/api/veiculos/{criado.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<VeiculoDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
        Assert.Equal(criado.Placa, resultado.Placa);
    }

    [Fact]
    public async Task GetVeiculoPorId_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.GetAsync("/api/veiculos/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTodosVeiculos_DeveRetornar200EConterVeiculoCriado()
    {
        var criado = await CriarVeiculoAsync("CCC3C33", "9BWZZZ377VT004263");

        var response = await _client.GetAsync("/api/veiculos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<List<VeiculoDto>>();

        Assert.NotNull(resultado);
        Assert.Contains(resultado!, v => v.Id == criado.Id);
    }

    [Fact]
    public async Task PutVeiculo_ComDadosValidos_DeveRetornar200()
    {
        var criado = await CriarVeiculoAsync("DDD4D44", "9BWZZZ377VT004264");

        var dto = CriarAtualizacaoDto("EEE5E55", "9BWZZZ377VT004265");

        var response = await _client.PutAsJsonAsync($"/api/veiculos/{criado.Id}", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<VeiculoDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
        Assert.Equal(dto.Marca, resultado.Marca);
        Assert.Equal(dto.Modelo, resultado.Modelo);
        Assert.Equal(dto.Placa, resultado.Placa);
        Assert.Equal(dto.Chassi, resultado.Chassi);
        Assert.Equal(dto.Quilometragem, resultado.Quilometragem);
    }

    [Fact]
    public async Task PutVeiculo_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.PutAsJsonAsync(
            "/api/veiculos/999999",
            CriarAtualizacaoDto("FFF6F66", "9BWZZZ377VT004266"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteVeiculo_ComIdExistente_DeveRetornar204()
    {
        var criado = await CriarVeiculoAsync("GGG7G77", "9BWZZZ377VT004267");

        var response = await _client.DeleteAsync($"/api/veiculos/{criado.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/veiculos/{criado.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteVeiculo_ComIdInexistente_DeveRetornar404()
    {
        await AutenticarAsync();

        var response = await _client.DeleteAsync("/api/veiculos/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
