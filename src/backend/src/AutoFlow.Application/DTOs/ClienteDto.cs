using AutoFlow.Application.Interfaces.DTOs;

namespace AutoFlow.Application.DTOs
{
    public record ClienteDto(
        int Id,
        string Nome,
        string Documento,
        string Telefone,
        string Email
    );

    public record CriaClienteDto(
        string Nome,
        string Documento,
        string Telefone,
        string Email
    ) : IClienteDTO;

    public record AtualizaClienteDto(
        string Nome,
        string Documento,
        string Telefone,
        string Email
    ) : IClienteDTO;
}
