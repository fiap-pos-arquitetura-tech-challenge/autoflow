using AutoFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Domain.ValueObjects
{
    public sealed class Chassi
    {
        public string Valor { get; }

        public Chassi(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ChassiInvalidoException("Chassi inválido.");

            if (valor.Length != 17)
                throw new ChassiInvalidoException("Chassi deve possuir 17 caracteres.");

            Valor = valor.ToUpper();
        }

        public override string ToString()
            => Valor;
    }
}
