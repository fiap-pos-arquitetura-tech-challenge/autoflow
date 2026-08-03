using AutoFlow.Api.Extensions;
using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Services;

namespace AutoFlow.Api.Endpoints
{
    public static class ClienteEndpoints
    {
        public static IEndpointRouteBuilder MapClienteEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/clientes")
                .WithTags("Categories");

            app.MapPost("/", Adicionar)
                .Produces<ClienteDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            app.MapPut("/{id:int}", Atualizar)
                .Produces<ClienteDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);
            app.MapDelete("/{id:int}", Excluir)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            app.MapGet("/{id:int}", ObterPorId)
                .Produces<ClienteDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            app.MapGet("/", ObterTodos)
                .Produces<IEnumerable<ClienteDto>>(StatusCodes.Status200OK);
            return app;
        }

        private static async Task<IResult> Excluir(
            int id, 
            IClienteService clienteService)
        {
            var result = await clienteService.ExcluirAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> Adicionar(
            CriaClienteDto clienteDto,
            IClienteService clienteService)
        {
            var result = await clienteService.AdicionarAsync(clienteDto);

            return result.ToCreatedHttpResult(c => $"/clientes/{c.Id}");
        }

        private static async Task<IResult> Atualizar(
            int id,
            AtualizaClienteDto clienteDto,
            IClienteService clienteService)
        {
            var result = await clienteService.AtualizarAsync(id, clienteDto);

            return result.ToHttpResult();
        }

        private static async Task<IResult> ObterPorId(
            int id,
            IClienteService clienteService)
        {
            var result = await clienteService.ObterPorIdAsync(id);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ObterTodos(
            IClienteService clienteService)
        {
            var clientes = await clienteService.ObterTodosAsync();
            return Results.Ok(clientes);
        }
    }
}
