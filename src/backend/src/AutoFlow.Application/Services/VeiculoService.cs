using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;
using AutoFlow.Domain.Models;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.Application.Services
{
    public class VeiculoService(IVeiculoRepositorio veiculoRepositorio) : IVeiculoService
    {
        private readonly IVeiculoRepositorio _veiculoRepositorio = veiculoRepositorio;
        public async Task<Result<VeiculoDto>> AdicionarAsync(CriaVeiculoDto veiculoDto)
        {
            var validador = VeiculoValidator.Validar(veiculoDto);

            if (validador is not null)
            {
                return Result<VeiculoDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var veiculo = new Veiculo(
                veiculoDto.ClienteId,
                veiculoDto.Marca,
                veiculoDto.Modelo,
                veiculoDto.AnoFabricacao,
                veiculoDto.AnoModelo,
                veiculoDto.Cor,
                veiculoDto.Tipo,
                veiculoDto.Combustivel,
                new Placa(veiculoDto.Placa),
                new Chassi(veiculoDto.Chassi),
                new Quilometragem(veiculoDto.Quilometragem)
            );

            await _veiculoRepositorio.AdicionarAsync(veiculo);

            return Result<VeiculoDto>.Success(new VeiculoDto(
                veiculo.Id,
                veiculo.ClienteId,
                veiculo.Marca,
                veiculo.Modelo,
                veiculo.AnoFabricacao,
                veiculo.AnoModelo,
                veiculo.Cor,
                veiculo.Tipo,
                veiculo.Combustivel,
                veiculo.Placa.Valor,
                veiculo.Chassi.Valor,
                veiculo.Quilometragem.Valor
            ));                
        }

        public async Task<Result<VeiculoDto>> AtualizarAsync(int id, AtualizaVeiculoDto veiculoDto)
        {
            var validador = VeiculoValidator.Validar(veiculoDto);

            if (validador is not null)
            {
                return Result<VeiculoDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var veiculo = await _veiculoRepositorio.ObterPorIdAsync(id);

            if(veiculo is null)
            {
                return Result<VeiculoDto>.Failure("Veiculo não encontrado!", ErrorType.NotFound);
            }

            veiculo.Atualizar(veiculoDto.ClienteId,
                veiculoDto.Marca,
                veiculoDto.Modelo,
                veiculoDto.AnoFabricacao,
                veiculo.AnoModelo,
                veiculo.Cor,
                veiculo.Tipo,
                veiculo.Combustivel,
                veiculo.Placa.Valor,
                veiculo.Chassi.Valor,
                veiculo.Quilometragem.Valor
            );

            await _veiculoRepositorio.AtualizarAsync(veiculo);

            return Result<VeiculoDto>.Success(new VeiculoDto(
                veiculo.Id,
                veiculo.ClienteId,
                veiculo.Marca,
                veiculo.Modelo,
                veiculo.AnoFabricacao,
                veiculo.AnoModelo,
                veiculo.Cor,
                veiculo.Tipo,
                veiculo.Combustivel,
                veiculo.Placa.Valor,
                veiculo.Chassi.Valor,
                veiculo.Quilometragem.Valor
            ));

        }

        public async Task<Result> ExcluirAsync(int id)
        {
            var veiculo = await _veiculoRepositorio.ObterPorIdAsync(id);

            if (veiculo is null)
            {
                return Result<VeiculoDto>.Failure("Veiculo não encontrado!", ErrorType.NotFound);
            }

            await _veiculoRepositorio.ExcluirAsync(veiculo);

            return Result.Success();
        }

        public async Task<Result<VeiculoDto>> ObterPorIdAsync(int id)
        {
            var veiculo = await _veiculoRepositorio.ObterPorIdAsync(id);

            if (veiculo is null)
            {
                return Result<VeiculoDto>.Failure("Veiculo não encontrado!", ErrorType.NotFound);
            }

            return Result<VeiculoDto>.Success(new VeiculoDto(
                veiculo.Id,
                veiculo.ClienteId,
                veiculo.Marca,
                veiculo.Modelo,
                veiculo.AnoFabricacao,
                veiculo.AnoModelo,
                veiculo.Cor,
                veiculo.Tipo,
                veiculo.Combustivel,
                veiculo.Placa.Valor,
                veiculo.Chassi.Valor,
                veiculo.Quilometragem.Valor
            ));

        }

        public async Task<IEnumerable<VeiculoDto>> ObterTodosAsync()
        {
            var veiculos = await _veiculoRepositorio.ObterTodosAsync();

            return veiculos.Select(veiculo => new VeiculoDto(
                veiculo.Id,
                veiculo.ClienteId,
                veiculo.Marca,
                veiculo.Modelo,
                veiculo.AnoFabricacao,
                veiculo.AnoModelo,
                veiculo.Cor,
                veiculo.Tipo,
                veiculo.Combustivel,
                veiculo.Placa.Valor,
                veiculo.Chassi.Valor,
                veiculo.Quilometragem.Valor
            ));
        }
    }
}
