using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.Domain.Models
{
    public class Cliente : BaseModel
    {
        public string Nome { get; private set; }
        public Documento Documento { get; private set; }
        public Telefone Telefone { get; private set; }
        public Email Email { get; private set; }

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

            Nome = nome;
            Documento = new Documento(documento);
            Telefone = new Telefone(telefone);
            Email = new Email(email);
        }

        public void Atualizar(string nome, string documento, string telefone, string email)
        {
            ValidarNome(nome);

            Nome = nome;
            Documento = new Documento(documento);
            Telefone = new Telefone(telefone);
            Email = new Email(email);
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ClienteInvalidoException("Nome é obrigatório.");
        }
    }
}
