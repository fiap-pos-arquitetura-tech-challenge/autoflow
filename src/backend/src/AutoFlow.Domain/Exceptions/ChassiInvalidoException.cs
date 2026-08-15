using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Domain.Exceptions
{
    public class ChassiInvalidoException(string? message) : Exception(message)
    {
    }
}
