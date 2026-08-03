using AutoFlow.Application.Interfaces.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Application.Validators
{
    public static class ClienteValidador
    {
        public static Result? Validar(IClienteDTO cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                return Result.Failure("Nome é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(cliente.Documento))
                return Result.Failure("Documento é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(cliente.Telefone))
                return Result.Failure("Telefone é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(cliente.Email))
                return Result.Failure("Email é obrigatório.", ErrorType.Validation);

            return null;
        }
    }
}
