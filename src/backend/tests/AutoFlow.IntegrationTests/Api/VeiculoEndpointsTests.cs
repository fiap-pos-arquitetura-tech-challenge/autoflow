
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

    private static CriaVeiculoDto CriarVeiculoDto()
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
            Placa: "ABC1D23",
            Chassi: "9BWZZZ377VT004251",
            Quilometragem: 10000
        );
    }

    private static AtualizaVeiculoDto CriarAtualizacaoDto()
    {
        return new AtualizaVeiculoDto(
            ClienteId: 1,
            Marca: "Honda",
            Modelo: "Civic",
            AnoFabricacao: 2022,
            AnoModelo: 2023,
            Cor: "Branco",
            Tipo: TipoVeiculo.Carro,
            Combustivel: Combustivel.Etanol,
            Placa: "XYZ9A99",
            Chassi: "9BWZZZ377VT004252",
            Quilometragem: 20000
        );
    }

    [Fact]
    public async Task PostVeiculo_ComDadosValidos_DeveRetornar201()
    {
        var token = await AuthHelper.LoginColaboradorAsync(
            _client,
            AutoFlowApplicationFactory.ColaboradorEmail,
            AutoFlowApplicationFactory.ColaboradorSenha);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = CriarVeiculoDto();

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
}
