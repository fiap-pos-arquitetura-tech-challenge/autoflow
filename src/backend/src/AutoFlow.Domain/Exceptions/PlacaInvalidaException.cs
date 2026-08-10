using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Domain.Exceptions
{
    public class PlacaInvalidaException(string? message) : Exception(message)
    {
    }
}
