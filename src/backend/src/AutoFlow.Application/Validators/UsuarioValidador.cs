using AutoFlow.Application.Interfaces.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Application.Validators
{
    public static class UsuarioValidador
    {
        public static Result? Validar(IUsuarioDTO usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nome))
                return Result.Failure("Nome é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(usuario.Email))
                return Result.Failure("Email é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(usuario.Senha))
                return Result.Failure("Senha é obrigatória.", ErrorType.Validation);

            return null;
        }
    }
}
