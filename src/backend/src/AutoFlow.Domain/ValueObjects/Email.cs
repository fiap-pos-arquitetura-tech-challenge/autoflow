using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.ValueObjects
{
    public class Email
    {
        public string Endereco { get; }

        public Email(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new EmailInvalidoException("Email é obrigatório.");

            Endereco = endereco;
        }
    }
}
