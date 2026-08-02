namespace AutoFlow.Domain.Exceptions
{
    public class DocumentoInvalidoException : Exception
    {
        public DocumentoInvalidoException(string? message) : base(message)
        { 
        }
    }
}
