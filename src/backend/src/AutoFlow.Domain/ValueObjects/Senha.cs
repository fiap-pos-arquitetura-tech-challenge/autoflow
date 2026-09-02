using System.Security.Cryptography;
using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.ValueObjects
{
    public class Senha
    {
        private const int TamanhoMinimo = 8;
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;
        private const int Iteracoes = 100_000;

        public string Hash { get; }
        public string Salt { get; }

        private Senha(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        public static Senha Criar(string senhaEmTexto)
        {
            if (string.IsNullOrWhiteSpace(senhaEmTexto))
                throw new SenhaInvalidaException("Senha é obrigatória.");

            if (senhaEmTexto.Length < TamanhoMinimo)
                throw new SenhaInvalidaException($"Senha deve ter no mínimo {TamanhoMinimo} caracteres.");

            var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(senhaEmTexto, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);

            return new Senha(Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public bool Verificar(string senhaEmTexto)
        {
            var salt = Convert.FromBase64String(Salt);
            var hashInformado = Rfc2898DeriveBytes.Pbkdf2(senhaEmTexto, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
            var hashArmazenado = Convert.FromBase64String(Hash);

            return CryptographicOperations.FixedTimeEquals(hashInformado, hashArmazenado);
        }
    }
}
