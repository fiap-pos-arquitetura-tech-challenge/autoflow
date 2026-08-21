using System.Text.RegularExpressions;
using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.ValueObjects
{
    public partial class Email
    {
        public string Endereco { get; }

        [GeneratedRegex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$")]
        private static partial Regex EnderecoValidoRegex();

        public Email(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new EmailInvalidoException("Email é obrigatório.");

            if (!EnderecoValidoRegex().IsMatch(endereco))
                throw new EmailInvalidoException("Email inválido.");

            Endereco = endereco;
        }
    }
}
