using AutoFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Domain.ValueObjects
{
    public sealed class Quilometragem
    {
        public int Valor { get; }

        public Quilometragem(int valor)
        {
            if (valor < 0)
                throw new QuilometragemInvalidaException("Quilometragem inválida.");

            Valor = valor;
        }

        public override string ToString()
            => $"{Valor} km";
    }
}
