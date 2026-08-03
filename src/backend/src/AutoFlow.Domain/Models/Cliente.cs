using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.Domain.Models
{
    public class Cliente : BaseModel
    {
        public string Nome { get; private set; }
        public Documento Documento { get; private set; }
        public string Telefone { get; private set; }
        public string Email { get; private set; }

        public Cliente()
        {
            Nome = null!;
            Documento = null!;
            Telefone = null!;
            Email = null!;
        }

        public Cliente(string nome, string documento, string telefone, string email)
        {
            ValidarNome(nome);
            ValidarTelefone(telefone);
            ValidarEmail(email);

            Nome = nome;
            Documento = new Documento(documento);
            Telefone = telefone;
            Email = email;
        }

        public void Atualizar(string nome, string documento, string telefone, string email)
        {
            ValidarNome(nome);
            ValidarTelefone(telefone);
            ValidarEmail(email);

            Nome = nome;
            Documento = new Documento(documento);
            Telefone = telefone;
            Email = email;
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ClienteInvalidoException("Email é obrigatório.");
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ClienteInvalidoException("Nome é obrigatório.");
        }

        private static void ValidarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                throw new ClienteInvalidoException("Telefone é obrigatório.");
        }
    }
}
