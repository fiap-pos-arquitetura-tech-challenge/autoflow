using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;
using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Models;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.Application.Services
{
    public class UsuarioService(
        IUsuarioRepositorio usuarioRepositorio,
        IClienteRepositorio clienteRepositorio,
        ITokenService tokenService) : IUsuarioService
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio = usuarioRepositorio;
        private readonly IClienteRepositorio _clienteRepositorio = clienteRepositorio;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<UsuarioDto>> AdicionarColaboradorAsync(CriaColaboradorDto usuarioDto)
        {
            var validador = UsuarioValidador.Validar(usuarioDto);

            if (validador is not null)
            {
                return Result<UsuarioDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var usuarioComMesmoEmail = await _usuarioRepositorio.ObterPorEmailAsync(usuarioDto.Email);

            if (usuarioComMesmoEmail != null)
            {
                return Result<UsuarioDto>.Failure("Já existe um usuário cadastrado com este email.", ErrorType.Conflict);
            }

            var usuario = new Usuario(usuarioDto.Nome, usuarioDto.Email, usuarioDto.Senha, Perfil.Colaborador);

            await _usuarioRepositorio.AdicionarAsync(usuario);

            return Result<UsuarioDto>.Success(MapearParaDto(usuario));
        }

        public async Task<Result<UsuarioDto>> AtivarAcessoClienteAsync(AtivaAcessoClienteDto ativaAcessoDto)
        {
            var validador = AtivaAcessoClienteValidador.Validar(ativaAcessoDto);

            if (validador is not null)
            {
                return Result<UsuarioDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var documentoNormalizado = new Documento(ativaAcessoDto.Documento).Numero;
            var cliente = await _clienteRepositorio.ObterPorDocumentoAsync(documentoNormalizado);

            if (cliente == null)
            {
                return Result<UsuarioDto>.Failure("Cliente não encontrado.", ErrorType.NotFound);
            }

            var usuarioComMesmoEmail = await _usuarioRepositorio.ObterPorEmailAsync(cliente.Email.Endereco);

            if (usuarioComMesmoEmail != null)
            {
                return Result<UsuarioDto>.Failure("Acesso já foi ativado para este cliente.", ErrorType.Conflict);
            }

            var usuario = new Usuario(cliente.Nome, cliente.Email.Endereco, ativaAcessoDto.Senha, Perfil.Cliente, cliente.Id);

            await _usuarioRepositorio.AdicionarAsync(usuario);

            return Result<UsuarioDto>.Success(MapearParaDto(usuario));
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto loginDto)
        {
            var validador = LoginValidador.Validar(loginDto);

            if (validador is not null)
            {
                return Result<LoginResponseDto>.Failure(validador.Error!, validador.ErrorType!);
            }

            var usuario = await _usuarioRepositorio.ObterPorEmailAsync(loginDto.Email);

            if (usuario == null || !usuario.Senha.Verificar(loginDto.Senha))
            {
                return Result<LoginResponseDto>.Failure("Credenciais inválidas.", ErrorType.Unauthorized);
            }

            var token = _tokenService.GerarToken(usuario);

            return Result<LoginResponseDto>.Success(new LoginResponseDto(
                token.Token,
                token.ExpiraEm,
                usuario.Nome,
                usuario.Email.Endereco,
                usuario.Perfil,
                usuario.ClienteId
            ));
        }

        private static UsuarioDto MapearParaDto(Usuario usuario) => new(
            usuario.Id,
            usuario.Nome,
            usuario.Email.Endereco,
            usuario.Perfil,
            usuario.ClienteId
        );
    }
}
