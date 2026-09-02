using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.Models
{
    public class Servico : BaseModel
    {
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int TempoMedio { get; set; }

        public Servico()
        {
            Nome = null!;
            Preco = 0;
            TempoMedio = 0;
        }
        public Servico(string nome, decimal preco, int tempoMedio)
        {
            Nome = nome;
            Preco = preco;
            TempoMedio = tempoMedio;
        }

        public void Atualizar(string nome, decimal preco, int tempoMedio)
        {
            ValidarNome(nome);

            Nome = nome;
            Preco = preco;
            TempoMedio = tempoMedio;
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ServicoInvalidoException("Nome do serviço inválido");

            if (nome.All(char.IsDigit))
                throw new ServicoInvalidoException("Nome do serviço inválido");
        }


    }
}
     
