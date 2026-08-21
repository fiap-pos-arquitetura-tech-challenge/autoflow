using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Application.Validators
{
    public static class AtivaAcessoClienteValidador
    {
        public static Result? Validar(AtivaAcessoClienteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Documento))
                return Result.Failure("Documento é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(dto.Email))
                return Result.Failure("Email é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(dto.Senha))
                return Result.Failure("Senha é obrigatória.", ErrorType.Validation);

            return null;
        }
    }
}
