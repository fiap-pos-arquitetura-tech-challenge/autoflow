using System.Text.RegularExpressions;
using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.ValueObjects
{
    public partial class Telefone
    {
        public string Numero { get; }

        [GeneratedRegex(@"\D")]
        private static partial Regex NaoDigitoRegex();

        public Telefone(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new TelefoneInvalidoException("Telefone é obrigatório.");

            var numeroSemMascara = NaoDigitoRegex().Replace(numero, "");

            if (numeroSemMascara.Length is < 10 or > 11)
                throw new TelefoneInvalidoException("Telefone precisa ter 10 ou 11 dígitos, incluindo o DDD.");

            Numero = numeroSemMascara;
        }
    }
}
