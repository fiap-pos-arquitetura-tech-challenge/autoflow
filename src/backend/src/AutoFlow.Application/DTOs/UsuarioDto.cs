using AutoFlow.Application.Interfaces.DTOs;
using AutoFlow.Domain.Enums;

namespace AutoFlow.Application.DTOs
{
    public record UsuarioDto(
        int Id,
        string Nome,
        string Email,
        Perfil Perfil,
        int? ClienteId
    );

    public record CriaColaboradorDto(
        string Nome,
        string Email,
        string Senha
    ) : IUsuarioDTO;

    public record AtivaAcessoClienteDto(
        string Documento,
        string Email,
        string Senha
    );

    public record LoginRequestDto(
        string Email,
        string Senha
    );

    public record LoginResponseDto(
        string Token,
        DateTime ExpiraEm,
        string Nome,
        string Email,
        Perfil Perfil,
        int? ClienteId
    );
}
