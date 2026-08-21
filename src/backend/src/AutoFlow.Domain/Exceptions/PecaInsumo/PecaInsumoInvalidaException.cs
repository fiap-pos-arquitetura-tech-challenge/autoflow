using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Domain.Exceptions.PecaInsumo
{
    public class PecaInsumoInvalidaException(string? message) : Exception(message)
    {
    }
}
