namespace AutoFlow.Domain.Validators
{
    public static class CnpjValidador
    {
        private const int TamanhoCnpj = 14;
        private static readonly int[] Multiplicadores = [2, 3, 4, 5, 6, 7, 8, 9 ];

        public static bool Validar(string cnpj)
        {
            var normalizado = ApenasCaracteresValidosCnpj(cnpj);
            if (!TemTamanhoValido(normalizado)) return false;
            if (!TemCaracteresValidos(normalizado)) return false;
            if (TemSequenciaRepetida(normalizado)) return false;
            return TemDigitosValidos(normalizado);
        }

        private static string ApenasCaracteresValidosCnpj(string cnpj)
        {
            if (cnpj == null) return "";
            return new([.. cnpj.ToUpperInvariant().Where(char.IsLetterOrDigit)]);
        }

        private static bool TemTamanhoValido(string valor) => valor.Length == TamanhoCnpj;

        // Apenas os dígitos verificadores (posições 12–13) precisam ser numéricos.
        private static bool TemCaracteresValidos(string valor)
        {
            for (int i = TamanhoCnpj - 2; i < TamanhoCnpj; i++)
                if (!char.IsDigit(valor[i])) return false;
            return true;
        }

        private static bool TemSequenciaRepetida(string valor) =>
            valor.Distinct().Count() == 1;

        // ASCII - 48: '0'=0..'9'=9, 'A'=17..'Z'=42 (conforme IN RFB 2.229/2024)
        private static int ValorCaractere(char c) => c - '0';

        private static string CalcularDigito(string trecho)
        {
            var soma = 0;
            var m = 0;
            for (int i = trecho.Length - 1; i >= 0; i--)
            {
                soma += ValorCaractere(trecho[i]) * Multiplicadores[m];
                if (++m >= Multiplicadores.Length) m = 0;
            }
            var resto = soma % 11;
            return resto < 2 ? "0" : (11 - resto).ToString();
        }

        private static bool TemDigitosValidos(string valor)
        {
            var raiz = valor[..(TamanhoCnpj - 2)];
            var d1 = CalcularDigito(raiz);
            var d2 = CalcularDigito(raiz + d1);
            return d1 + d2 == valor[(TamanhoCnpj - 2)..];
        }
    }
}
