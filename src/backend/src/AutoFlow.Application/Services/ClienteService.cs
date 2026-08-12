using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;
using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Services
{
    public class ClienteService(IClienteRepositorio clienteRepositorio) : IClienteService
    {
        private readonly IClienteRepositorio _clienteRepositorio = clienteRepositorio;

        public async Task<Result<ClienteDto>> AdicionarAsync(CriaClienteDto clienteDto)
        {
            var validador = ClienteValidador.Validar(clienteDto);

            if (validador is not null)
            {
                return Result<ClienteDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var cliente = new Cliente(
                clienteDto.Nome,
                clienteDto.Documento,
                clienteDto.Telefone,
                clienteDto.Email
            );

            await _clienteRepositorio.AdicionarAsync(cliente);

            return Result<ClienteDto>.Success(new ClienteDto(
                cliente.Id,
                cliente.Nome,
                cliente.Documento.Numero,
                cliente.Telefone,
                cliente.Email
            ));
        }

        public async Task<Result<ClienteDto>> AtualizarAsync(int id, AtualizaClienteDto clienteDto)
        {
            var validador = ClienteValidador.Validar(clienteDto);

            if (validador is not null)
            {
                return Result<ClienteDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var cliente = await _clienteRepositorio.ObterPorIdAsync(id);

            if (cliente == null)
            {
                return Result<ClienteDto>.Failure("Cliente não encontrado.", ErrorType.NotFound);
            }

            cliente.Atualizar(
                clienteDto.Nome,
                clienteDto.Documento,
                clienteDto.Telefone,
                clienteDto.Email
            );

            await _clienteRepositorio.AtualizarAsync(cliente);

            return Result<ClienteDto>.Success(new ClienteDto(
                cliente.Id,
                cliente.Nome,
                cliente.Documento.Numero,
                cliente.Telefone,
                cliente.Email
            ));
        }

        public async Task<Result> ExcluirAsync(int id)
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(id);

            if (cliente == null)
            {
                return Result.Failure("Cliente não encontrado.", ErrorType.NotFound);
            }

            await _clienteRepositorio.ExcluirAsync(cliente);

            return Result.Success();
        }

        public async Task<Result<ClienteDto>> ObterPorIdAsync(int id)
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(id);

            if (cliente == null)
            {
                return Result<ClienteDto>.Failure("Cliente não encontrado.", ErrorType.NotFound);
            }

            return Result<ClienteDto>.Success(new ClienteDto(
                cliente.Id,
                cliente.Nome,
                cliente.Documento.Numero,
                cliente.Telefone,
                cliente.Email
            ));
        }

        public async Task<IEnumerable<ClienteDto>> ObterTodosAsync()
        {
            var clientes = await _clienteRepositorio.ObterTodosAsync();
            return clientes.Select(c => new ClienteDto
            (
                c.Id,
                c.Nome,
                c.Documento.Numero,
                c.Telefone,
                c.Email
            ));
        }
    }
}
