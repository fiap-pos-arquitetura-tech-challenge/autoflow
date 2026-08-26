using AutoFlow.Api.Extensions;
using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Domain.Enums;
using System.Security.Claims;

namespace AutoFlow.Api.Endpoints
{
    public static class OrdemServicoEndpoints
    {
        public static IEndpointRouteBuilder MapOrdemServicoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/ordens-servico")
                .RequireAuthorization(policy => policy.RequireRole(nameof(Perfil.Colaborador)))
                .WithTags("Ordens de Serviço");

            group.MapPost("/", Adicionar)
                .Produces<OrdemServicoDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/", ObterTodos)
                .Produces<IEnumerable<OrdemServicoDto>>(StatusCodes.Status200OK);

            group.MapGet("/{id:int}", ObterPorId)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/{id:int}/avarias", RegistrarAvarias)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/diagnostico/iniciar", IniciarDiagnostico)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPut("/{id:int}/diagnostico", RegistrarDiagnostico)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/servicos", AdicionarServico)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapDelete("/{id:int}/servicos/{servicoId:int}", RemoverServico)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/pecas", AdicionarPeca)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapDelete("/{id:int}/pecas/{pecaId:int}", RemoverPeca)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/orcamento", GerarOrcamento)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/servicos/{itemServicoId:int}/execucao/iniciar", IniciarExecucaoServico)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/servicos/{itemServicoId:int}/execucao/finalizar", FinalizarExecucaoServico)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/finalizar", Finalizar)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/entregar", Entregar)
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            app.MapGet("/api/ordens-servico/{id:int}/orcamento", ConsultarOrcamento)
                .RequireAuthorization(policy => policy.RequireRole(nameof(Perfil.Cliente)))
                .WithTags("Ordens de Serviço")
                .Produces<OrcamentoClienteDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/api/ordens-servico/{id:int}/orcamento/aprovar", AprovarOrcamento)
                .RequireAuthorization(policy => policy.RequireRole(nameof(Perfil.Cliente)))
                .WithTags("Ordens de Serviço")
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            app.MapPost("/api/ordens-servico/{id:int}/orcamento/reprovar", ReprovarOrcamento)
                .RequireAuthorization(policy => policy.RequireRole(nameof(Perfil.Cliente)))
                .WithTags("Ordens de Serviço")
                .Produces<OrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            app.MapGet("/api/ordens-servico/{id:int}/andamento", ConsultarAndamento)
                .RequireAuthorization(policy => policy.RequireRole(nameof(Perfil.Colaborador), nameof(Perfil.Cliente)))
                .WithTags("Ordens de Serviço")
                .Produces<AndamentoOrdemServicoDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound);

            return app;
        }

        private static async Task<IResult> Adicionar(CriaOrdemServicoDto ordemServicoDto, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.AdicionarAsync(ordemServicoDto);
            return result.ToCreatedHttpResult(o => $"/api/ordens-servico/{o.Id}");
        }

        private static async Task<IResult> ObterTodos(IOrdemServicoService ordemServicoService)
        {
            var ordens = await ordemServicoService.ObterTodosAsync();
            return Results.Ok(ordens);
        }

        private static async Task<IResult> ObterPorId(int id, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.ObterPorIdAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> RegistrarAvarias(int id, RegistraAvariasOrdemServicoDto ordemServicoDto, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.RegistrarAvariasAsync(id, ordemServicoDto);
            return result.ToHttpResult();
        }

        private static async Task<IResult> IniciarDiagnostico(int id, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.IniciarDiagnosticoAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> RegistrarDiagnostico(int id, RegistraDiagnosticoOrdemServicoDto ordemServicoDto, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.RegistrarDiagnosticoAsync(id, ordemServicoDto);
            return result.ToHttpResult();
        }

        private static async Task<IResult> AdicionarServico(int id, AdicionaServicoOrdemServicoDto ordemServicoDto, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.AdicionarServicoAsync(id, ordemServicoDto);
            return result.ToHttpResult();
        }

        private static async Task<IResult> RemoverServico(int id, int servicoId, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.RemoverServicoAsync(id, servicoId);
            return result.ToHttpResult();
        }

        private static async Task<IResult> AdicionarPeca(int id, AdicionaPecaOrdemServicoDto ordemServicoDto, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.AdicionarPecaAsync(id, ordemServicoDto);
            return result.ToHttpResult();
        }

        private static async Task<IResult> RemoverPeca(int id, int pecaId, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.RemoverPecaAsync(id, pecaId);
            return result.ToHttpResult();
        }

        private static async Task<IResult> GerarOrcamento(int id, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.GerarOrcamentoAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ConsultarOrcamento(int id, ClaimsPrincipal usuario, IOrdemServicoService ordemServicoService)
        {
            var clienteIdClaim = usuario.FindFirst("clienteId")?.Value;

            if (!int.TryParse(clienteIdClaim, out var clienteId))
                return Results.Unauthorized();

            var result = await ordemServicoService.ConsultarOrcamentoClienteAsync(id, clienteId);
            return result.ToHttpResult();
        }

        private static async Task<IResult> AprovarOrcamento(int id, ClaimsPrincipal usuario, IOrdemServicoService ordemServicoService)
        {
            var clienteIdClaim = usuario.FindFirst("clienteId")?.Value;

            if (!int.TryParse(clienteIdClaim, out var clienteId))
                return Results.Unauthorized();

            var result = await ordemServicoService.AprovarOrcamentoAsync(id, clienteId);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ReprovarOrcamento(int id, ReprovaOrcamentoOrdemServicoDto ordemServicoDto, ClaimsPrincipal usuario, IOrdemServicoService ordemServicoService)
        {
            var clienteIdClaim = usuario.FindFirst("clienteId")?.Value;

            if (!int.TryParse(clienteIdClaim, out var clienteId))
                return Results.Unauthorized();

            var result = await ordemServicoService.ReprovarOrcamentoAsync(id, clienteId, ordemServicoDto);
            return result.ToHttpResult();
        }

        private static async Task<IResult> IniciarExecucaoServico(int id, int itemServicoId, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.IniciarExecucaoServicoAsync(id, itemServicoId);
            return result.ToHttpResult();
        }

        private static async Task<IResult> FinalizarExecucaoServico(int id, int itemServicoId, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.FinalizarExecucaoServicoAsync(id, itemServicoId);
            return result.ToHttpResult();
        }

        private static async Task<IResult> Finalizar(int id, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.FinalizarAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> Entregar(int id, IOrdemServicoService ordemServicoService)
        {
            var result = await ordemServicoService.EntregarAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ConsultarAndamento(int id, ClaimsPrincipal usuario, IOrdemServicoService ordemServicoService)
        {
            if (usuario.IsInRole(nameof(Perfil.Colaborador)))
            {
                var resultadoColaborador = await ordemServicoService.ConsultarAndamentoAsync(id);
                return resultadoColaborador.ToHttpResult();
            }

            var clienteIdClaim = usuario.FindFirst("clienteId")?.Value;

            if (!int.TryParse(clienteIdClaim, out var clienteId))
                return Results.Unauthorized();

            var resultadoCliente = await ordemServicoService.ConsultarAndamentoClienteAsync(id, clienteId);
            return resultadoCliente.ToHttpResult();
        }
    }
}
