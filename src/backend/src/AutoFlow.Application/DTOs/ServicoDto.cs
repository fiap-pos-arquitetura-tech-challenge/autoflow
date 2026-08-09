using AutoFlow.Application.Interfaces.DTOs;

namespace AutoFlow.Application.DTOs
{
    public record ServicoDto(
        int Id,
        string Nome,
        decimal Preco,
        int TempoMedio
    );

    public record CriaServicoDto(
           int Id,
           string Nome,
           decimal Preco,
           int TempoMedio
       ) : IServicoDTO;

    public record AtualizaServicoDto(
           int Id,
           string Nome,
           decimal Preco,
           int TempoMedio
       ) : IServicoDTO;
}
