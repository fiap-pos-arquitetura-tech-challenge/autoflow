namespace AutoFlow.Domain.Validators
{
    public class DigitoVerificador(string numero)
    {
        private string _numero = numero;
        private const int Modulo = 11;
        private readonly List<int> _multiplicadores = [ 2, 3, 4, 5, 6, 7, 8, 9 ];
        private readonly Dictionary<int, string> _substituicoes = [];
        private readonly bool _complementarDoModulo = true;

        public DigitoVerificador ComMultiplicadoresDeAte(int primeiroMultiplicador, int ultimoMultiplicador)
        {
            _multiplicadores.Clear();
            for (var i = primeiroMultiplicador; i <= ultimoMultiplicador; i++)
                _multiplicadores.Add(i);

            return this;
        }

        public DigitoVerificador Substituindo(string substituto, params int[] digitos)
        {
            foreach (var i in digitos)
            {
                _substituicoes[i] = substituto;
            }
            return this;
        }

        public void AdicionaDigito(string digito)
        {
            _numero = string.Concat(_numero, digito);
        }

        public string CalculaDigito()
        {
            return (_numero.Length <= 0) ? "" : ObterSomaDigito();
        }

        private string ObterSomaDigito()
        {
            var soma = 0;
            for (int i = _numero.Length - 1, m = 0; i >= 0; i--)
            {
                var produto = (int)char.GetNumericValue(_numero[i]) * _multiplicadores[m];
                soma += produto;

                if (++m >= _multiplicadores.Count) m = 0;
            }

            var mod = (soma % Modulo);
            var resultado = _complementarDoModulo ? Modulo - mod : mod;

            return _substituicoes.TryGetValue(resultado, out var substituto) ? substituto : resultado.ToString();
        }
    }
}
