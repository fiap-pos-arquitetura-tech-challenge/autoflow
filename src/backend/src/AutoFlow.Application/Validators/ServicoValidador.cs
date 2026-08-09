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
                return Result.Failure("Nome é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(servico.Preco.ToString()))
                return Result.Failure("Preço é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(servico.TempoMedio.ToString()))
                return Result.Failure("Tempo médio é obrigatório.", ErrorType.Validation);

            return null;
        }
    }
}
