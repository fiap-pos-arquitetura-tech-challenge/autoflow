using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Application.Validators
{
    public static class OrdemServicoValidador
    {
        public static Result? Validar(CriaOrdemServicoDto dto)
        {
            if (dto.ClienteId <= 0)
                return Result.Failure("Cliente é obrigatório.", ErrorType.Validation);

            if (dto.VeiculoId <= 0)
                return Result.Failure("Veículo é obrigatório.", ErrorType.Validation);

            return null;
        }

        public static Result? Validar(RegistraDiagnosticoOrdemServicoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Diagnostico))
                return Result.Failure("Diagnóstico é obrigatório.", ErrorType.Validation);

            return null;
        }

        public static Result? Validar(AdicionaServicoOrdemServicoDto dto)
        {
            if (dto.ServicoId <= 0)
                return Result.Failure("Serviço é obrigatório.", ErrorType.Validation);

            if (dto.Quantidade <= 0)
                return Result.Failure("Quantidade do serviço deve ser maior que zero.", ErrorType.Validation);

            return null;
        }

        public static Result? Validar(AdicionaPecaOrdemServicoDto dto)
        {
            if (dto.PecaId <= 0)
                return Result.Failure("Peça é obrigatória.", ErrorType.Validation);

            if (dto.Quantidade <= 0)
                return Result.Failure("Quantidade da peça deve ser maior que zero.", ErrorType.Validation);

            return null;
        }

        public static Result? Validar(ReprovaOrcamentoOrdemServicoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Justificativa))
                return Result.Failure("Justificativa da reprovação é obrigatória.", ErrorType.Validation);

            return null;
        }
    }
}
