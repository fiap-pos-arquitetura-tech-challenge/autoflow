using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.ValueObjects
{
    public class Telefone
    {
        public string Numero { get; }

        public Telefone(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new TelefoneInvalidoException("Telefone é obrigatório.");

            Numero = numero;
        }
    }
}
