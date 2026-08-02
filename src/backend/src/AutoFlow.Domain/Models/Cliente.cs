using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.Domain.Models
{
    public class Cliente : BaseModel
    {
        public string Nome { get; set; }
        public Documento Documento { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }

        public Cliente()
        {
            
        }

        public Cliente(string nome, Documento documento, string telefone, string email)
        {
            Nome = nome;
            Documento = documento;
            Telefone = telefone;
            Email = email;
        }
    }
}
