using AutoFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoFlow.Domain.ValueObjects
{
    public sealed class Placa
    {
        public string Valor { get; }

        public Placa(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new PlacaInvalidaException("Placa inválida.");

            valor = valor.ToUpper().Trim();

            var placaMercosul = @"^[A-Z]{3}[0-9][A-Z][0-9]{2}$";
            var placaAntiga = @"^[A-Z]{3}[0-9]{4}$";

            if (!Regex.IsMatch(valor, placaMercosul) &&
                !Regex.IsMatch(valor, placaAntiga))
            {
                throw new PlacaInvalidaException("Formato de placa inválido.");
            }

            Valor = valor;
        }

        public override string ToString()
            => Valor;
    }
}
