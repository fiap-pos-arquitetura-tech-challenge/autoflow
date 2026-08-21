using AutoFlow.Application.Interfaces.DTOs;

namespace AutoFlow.Application.DTOs;

public record PecaInsumoDto(
    int Id,
    string Nome,
    decimal Valor
);

public record CriaPecaInsumoDto(
    string Nome,
    decimal Valor
) : IPecaInsumoDTO;

public record AtualizaPecaInsumoDto(
    string Nome,
    decimal Valor
) : IPecaInsumoDTO;