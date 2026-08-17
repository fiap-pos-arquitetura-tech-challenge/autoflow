using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;
using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Services
{
    public class ServicoService(IServicoRepositorio servicoRepositorio) : IServicoService
    {
        private readonly IServicoRepositorio _servicoRepositorio = servicoRepositorio;

        public async Task<Result<ServicoDto>> AdicionarAsync(CriaServicoDto servicoDto)
        {
            var validador = ServicoValidador.Validar(servicoDto);

            if (validador is not null)
            {
                return Result<ServicoDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var nome = servicoDto.Nome.Trim();

            var servicoExistente = await _servicoRepositorio.ExistePorNomeAsync(nome);

            if (servicoExistente)
            {
                return Result<ServicoDto>.Failure("Esse serviço já está cadastrado.", ErrorType.Validation);
            }

            var servico = new Servico(
                nome,
                servicoDto.Preco,
                servicoDto.TempoMedio
            );

            await _servicoRepositorio.AdicionarAsync(servico);

            return Result<ServicoDto>.Success(new ServicoDto(
                servico.Id,
                servico.Nome,
                servico.Preco,
                servico.TempoMedio
            ));
        }

        public async Task<Result<ServicoDto>> AtualizarAsync(int id, AtualizaServicoDto servicoDto)
        {
            var validador = ServicoValidador.Validar(servicoDto);

            if (validador is not null)
            {
                return Result<ServicoDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var servico = await _servicoRepositorio.ObterPorIdAsync(id);

            if (servico == null)
            {
                return Result<ServicoDto>.Failure("Serviço não encontrado.", ErrorType.NotFound);
            }

            var nome = servicoDto.Nome.Trim();

            var existe = await _servicoRepositorio
                .ExistePorNomeAsync(nome, id);

            if (existe)
            {
                return Result<ServicoDto>.Failure(
                    "Esse serviço já está cadastrado.",
                    ErrorType.Validation);
            }

            servico.Atualizar(
                nome,
                servicoDto.Preco,
                servicoDto.TempoMedio
            );

            await _servicoRepositorio.AtualizarAsync(servico);

            return Result<ServicoDto>.Success(new ServicoDto(
                servico.Id,
                servico.Nome,
                servico.Preco,
                servico.TempoMedio
            ));
        }

        public async Task<Result> ExcluirAsync(int id)
        {
            var servico = await _servicoRepositorio.ObterPorIdAsync(id);

            if (servico == null)
            {
                return Result.Failure("Serviço não encontrado.", ErrorType.NotFound);
            }

            await _servicoRepositorio.ExcluirAsync(servico);

            return Result.Success();
        }

        public async Task<Result<ServicoDto>> ObterPorIdAsync(int id)
        {
            var servICO = await _servicoRepositorio.ObterPorIdAsync(id);

            if (servICO == null)
            {
                return Result<ServicoDto>.Failure("Serviço não encontrado.", ErrorType.NotFound);
            }

            return Result<ServicoDto>.Success(new ServicoDto(
                servICO.Id,
                servICO.Nome,
                servICO.Preco,
                servICO.TempoMedio
            ));
        }

        public async Task<IEnumerable<ServicoDto>> ObterTodosAsync()
        {
            var serviços = await _servicoRepositorio.ObterTodosAsync();
            return serviços.Select(s => new ServicoDto
            (
                s.Id,
                s.Nome,
                s.Preco,
                s.TempoMedio
            ));
        }
        public async Task<Result<ServicoDto>> ObterPorNomeAsync(string nome)
        {
            var servico = await _servicoRepositorio.ObterPorNomeAsync(nome);

            if (servico == null)
            {
                return Result<ServicoDto>.Failure(
                    "Serviço não encontrado.",
                    ErrorType.NotFound);
            }

            return Result<ServicoDto>.Success(
                new ServicoDto(
                    servico.Id,
                    servico.Nome,
                    servico.Preco,
                    servico.TempoMedio
                ));
        }
    }
}

