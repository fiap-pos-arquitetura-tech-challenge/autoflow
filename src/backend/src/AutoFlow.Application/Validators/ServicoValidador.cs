using AutoFlow.Application.Interfaces.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Application.Validators
{
    public static class ServicoValidador
    {
        public static Result? Validar(IServicoDTO servico)
        {
            if (string.IsNullOrWhiteSpace(servico.Nome))
                return Result.Failure(
                    "Nome é obrigatório.",
                    ErrorType.Validation);

            if (servico.Nome.All(char.IsDigit))
                return Result.Failure(
                    "Nome do serviço inválido",
                    ErrorType.Validation);

            if (servico.Preco <= 0)
                return Result.Failure(
                    "Preço deve ser maior que zero.",
                    ErrorType.Validation);

            if (servico.TempoMedio <= 0)
                return Result.Failure(
                    "Tempo médio deve ser maior que zero.",
                    ErrorType.Validation);


            return null;
        }
    }
}
