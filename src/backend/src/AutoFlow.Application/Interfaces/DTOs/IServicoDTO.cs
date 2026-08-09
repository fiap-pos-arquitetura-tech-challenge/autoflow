namespace AutoFlow.Application.Interfaces.DTOs
{
    public interface IServicoDTO
    {
        public int Id { get; }
        public string Nome { get; }
        public decimal Preco { get; }
        public int TempoMedio { get; }

    }
}
