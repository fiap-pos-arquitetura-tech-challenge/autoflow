using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Domain.Exceptions
{
    public class QuilometragemInvalidaException(string? message) : Exception(message)
    {
    }
}
