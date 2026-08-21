namespace AutoFlow.Domain.Exceptions;

public class EstoqueInvalidoException : Exception
{
    public EstoqueInvalidoException(string message)
        : base(message)
    {
    }
}