using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Validators;

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

            var numeroSemMascara = RemoverMascara(numero);

            if (!ValidarTamanhoDocumento(numeroSemMascara))
                throw new DocumentoInvalidoException("Documento precisa ter entre 11 e 14 caracteres.");

            Numero = numeroSemMascara;

            if (!ValidarDocumento(numeroSemMascara))
                throw new DocumentoInvalidoException($"{(EhCpf ? "CPF" : "CNPJ")} inválido.");
        }

        private static string RemoverMascara(string numero)
        {
            return new string([.. numero.Where(char.IsLetterOrDigit)]);
        }

        private static bool ValidarTamanhoDocumento(string numero)
        {
            return numero.Length == 11 || numero.Length == 14;
        }

        private bool ValidarDocumento(string numero)
        {
            return EhCpf ? CpfValidador.Validar(numero) : CnpjValidador.Validar(numero);
        }
    }
}
