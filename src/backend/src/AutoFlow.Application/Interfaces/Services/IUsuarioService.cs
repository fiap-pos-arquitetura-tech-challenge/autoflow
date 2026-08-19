using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;

namespace AutoFlow.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        public Task<Result<UsuarioDto>> AdicionarColaboradorAsync(CriaColaboradorDto usuarioDto);
        public Task<Result<UsuarioDto>> AtivarAcessoClienteAsync(AtivaAcessoClienteDto ativaAcessoDto);
        public Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto loginDto);
    }
}
