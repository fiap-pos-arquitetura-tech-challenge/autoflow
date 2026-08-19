using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Application.Validators
{
    public static class LoginValidador
    {
        public static Result? Validar(LoginRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return Result.Failure("Email é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(dto.Senha))
                return Result.Failure("Senha é obrigatória.", ErrorType.Validation);

            return null;
        }
    }
}
