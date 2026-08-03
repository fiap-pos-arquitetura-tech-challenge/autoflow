using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.ValueObjects
{
    public class Documento
    {
        public string Numero { get; }

        public bool EhCpf => Numero.Length == 11;

        public bool EhCnpj => Numero.Length == 14;

        public Documento(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new DocumentoInvalidoException("Número do documento é obrigatório.");

            if (!Validar(numero))
                throw new DocumentoInvalidoException("CPF/CNPJ inválido.");

            Numero = numero;
        }

        private static bool Validar(string numero)
        {
            return numero.Length == 11 || numero.Length == 14;
        }
    }
}
