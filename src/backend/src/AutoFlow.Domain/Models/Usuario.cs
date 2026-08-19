using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.Domain.Models
{
    public class Usuario : BaseModel
    {
        public string Nome { get; private set; }
        public Email Email { get; private set; }
        public Senha Senha { get; private set; }
        public Perfil Perfil { get; private set; }
        public int? ClienteId { get; private set; }

        public Usuario()
        {
            Nome = null!;
            Email = null!;
            Senha = null!;
        }

        public Usuario(string nome, string email, string senha, Perfil perfil, int? clienteId = null)
        {
            ValidarNome(nome);
            ValidarVinculoCliente(perfil, clienteId);

            Nome = nome;
            Email = new Email(email);
            Senha = Senha.Criar(senha);
            Perfil = perfil;
            ClienteId = clienteId;
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new UsuarioInvalidoException("Nome é obrigatório.");
        }

        private static void ValidarVinculoCliente(Perfil perfil, int? clienteId)
        {
            if (perfil == Perfil.Cliente && clienteId is null)
                throw new UsuarioInvalidoException("Usuário do perfil Cliente precisa estar vinculado a um Cliente.");

            if (perfil == Perfil.Colaborador && clienteId is not null)
                throw new UsuarioInvalidoException("Usuário do perfil Colaborador não deve estar vinculado a um Cliente.");
        }
    }
}
