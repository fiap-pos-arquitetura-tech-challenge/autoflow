namespace AutoFlow.Domain.Exceptions
{
    public abstract class DomainException(string? message) : Exception(message)
    {
        public virtual int StatusCode => 400;
    }
}
