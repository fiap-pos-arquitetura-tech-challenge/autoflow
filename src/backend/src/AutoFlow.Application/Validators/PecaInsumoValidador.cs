using AutoFlow.Application.Interfaces.DTOs;
using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Application.Validators
{
    public static class PecaInsumoValidador
    {
        public static Result? Validar(IPecaInsumoDTO pecaInsumo)
        {
            if (string.IsNullOrWhiteSpace(pecaInsumo.Nome))
                return Result.Failure("Nome é obrigatório.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(pecaInsumo.Valor.ToString()))
                return Result.Failure("Valor é obrigatório.", ErrorType.Validation);

            return null;
        }
    }
}
