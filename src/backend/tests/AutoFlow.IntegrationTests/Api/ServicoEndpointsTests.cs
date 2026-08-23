
using AutoFlow.Application.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace AutoFlow.IntegrationTests.Api;

public class ServicoEndpointsTests(AutoFlowApplicationFactory factory) : IClassFixture<AutoFlowApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CriaServicoDto CriarServicoDto(string nome)
    {
        return new CriaServicoDto(
            Nome: nome,
            Preco: 150.50m,
            TempoMedio: 60
        );
    }

    private static AtualizaServicoDto CriarAtualizacaoDto(string nome)
    {
        return new AtualizaServicoDto(
            Nome: nome,
            Preco: 250.75m,
            TempoMedio: 90
        );
    }

    private async Task<ServicoDto> CriarServicoAsync(string nome)
    {
        var response = await _client.PostAsJsonAsync("/api/servicos", CriarServicoDto(nome));

        response.EnsureSuccessStatusCode();

        var resultado = await response.Content.ReadFromJsonAsync<ServicoDto>();

        return resultado!;
    }

    [Fact]
    public async Task PostServico_ComDadosValidos_DeveRetornar201()
    {
        var dto = CriarServicoDto("Troca de óleo");

        var response = await _client.PostAsJsonAsync("/api/servicos", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<ServicoDto>();

        Assert.NotNull(resultado);

        Assert.True(resultado!.Id > 0);
        Assert.Equal(dto.Nome, resultado.Nome);
        Assert.Equal(dto.Preco, resultado.Preco);
        Assert.Equal(dto.TempoMedio, resultado.TempoMedio);
    }

    [Fact]
    public async Task PostServico_ComDadosInvalidos_DeveRetornar400()
    {
        var dto = CriarServicoDto("") ;

        var response = await _client.PostAsJsonAsync("/api/servicos", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostServico_ComNomeDuplicado_DeveRetornar400()
    {
        await CriarServicoAsync("Alinhamento e balanceamento");

        var response = await _client.PostAsJsonAsync("/api/servicos", CriarServicoDto("Alinhamento e balanceamento"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetServicoPorId_ComIdExistente_DeveRetornar200()
    {
        var criado = await CriarServicoAsync("Revisão de freios");

        var response = await _client.GetAsync($"/api/servicos/{criado.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<ServicoDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
        Assert.Equal(criado.Nome, resultado.Nome);
    }

    [Fact]
    public async Task GetServicoPorId_ComIdInexistente_DeveRetornar404()
    {
        var response = await _client.GetAsync("/api/servicos/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetServicoPorNome_ComNomeExistente_DeveRetornar200()
    {
        var criado = await CriarServicoAsync("Troca de pastilhas de freio");

        var response = await _client.GetAsync($"/api/servicos/nome/{Uri.EscapeDataString(criado.Nome)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<ServicoDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
    }

    [Fact]
    public async Task GetServicoPorNome_ComNomeInexistente_DeveRetornar404()
    {
        var response = await _client.GetAsync("/api/servicos/nome/ServicoQueNaoExiste");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTodosServicos_DeveRetornar200EConterServicoCriado()
    {
        var criado = await CriarServicoAsync("Higienização de ar condicionado");

        var response = await _client.GetAsync("/api/servicos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<List<ServicoDto>>();

        Assert.NotNull(resultado);
        Assert.Contains(resultado!, s => s.Id == criado.Id);
    }

    [Fact]
    public async Task PutServico_ComDadosValidos_DeveRetornar200()
    {
        var criado = await CriarServicoAsync("Rodízio de pneus");

        var dto = CriarAtualizacaoDto("Rodízio de pneus atualizado");

        var response = await _client.PutAsJsonAsync($"/api/servicos/{criado.Id}", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultado = await response.Content.ReadFromJsonAsync<ServicoDto>();

        Assert.NotNull(resultado);
        Assert.Equal(criado.Id, resultado!.Id);
        Assert.Equal(dto.Nome, resultado.Nome);
        Assert.Equal(dto.Preco, resultado.Preco);
        Assert.Equal(dto.TempoMedio, resultado.TempoMedio);
    }

    [Fact]
    public async Task PutServico_ComIdInexistente_DeveRetornar404()
    {
        var response = await _client.PutAsJsonAsync(
            "/api/servicos/999999",
            CriarAtualizacaoDto("Serviço inexistente"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutServico_ComDadosInvalidos_DeveRetornar400()
    {
        var criado = await CriarServicoAsync("Polimento");

        var dto = CriarAtualizacaoDto("Polimento") with { Preco = -10 };

        var response = await _client.PutAsJsonAsync($"/api/servicos/{criado.Id}", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteServico_ComIdExistente_DeveRetornar204()
    {
        var criado = await CriarServicoAsync("Diagnóstico eletrônico");

        var response = await _client.DeleteAsync($"/api/servicos/{criado.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/servicos/{criado.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteServico_ComIdInexistente_DeveRetornar404()
    {
        var response = await _client.DeleteAsync("/api/servicos/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
