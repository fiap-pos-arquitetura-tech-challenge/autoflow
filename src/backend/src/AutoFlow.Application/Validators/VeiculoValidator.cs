using AutoFlow.Application.Interfaces.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Application.Validators
{
    public static class VeiculoValidator
    { 
        public static Result? Validar(IVeiculoDTO veiculo)
        {
            if (veiculo.ClienteId <= 0)
                return Result.Failure("O Id do cliente é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(veiculo.Marca))
                return Result.Failure("Marca é obrigatória.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(veiculo.Modelo))
                return Result.Failure("Modelo é obrigatório.", ErrorType.Validation);

            if (veiculo.AnoFabricacao <= 1900)
                return Result.Failure("Ano de fabricação é obrigatório.", ErrorType.Validation);

            if (veiculo.AnoModelo <= 1900)
                return Result.Failure("Ano do modelo é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(veiculo.Cor))
                return Result.Failure("Cor é obrigatória.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(veiculo.Placa))
                return Result.Failure("Placa é obrigatória.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(veiculo.Chassi))
                return Result.Failure("Chassi é obrigatório.", ErrorType.Validation);

            if (veiculo.Quilometragem < 0)
                return Result.Failure("Quilometragem não pode ser negativa.", ErrorType.Validation);

            if(!Enum.IsDefined(veiculo.Tipo))
                return Result.Failure("Tipo de veículo é obrigatório.", ErrorType.Validation);

            if (!Enum.IsDefined(veiculo.Combustivel))
                return Result.Failure("Tipo de combustivél deve ser fornecido.", ErrorType.Validation);

            return null;
        }
    }
}
