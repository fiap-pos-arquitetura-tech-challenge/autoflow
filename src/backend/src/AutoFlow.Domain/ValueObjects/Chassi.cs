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
                throw new ArgumentException("Chassi inválido.");

            if (valor.Length != 17)
                throw new ArgumentException("Chassi deve possuir 17 caracteres.");

            Valor = valor.ToUpper();
        }

        public override string ToString()
            => Valor;
    }
}
