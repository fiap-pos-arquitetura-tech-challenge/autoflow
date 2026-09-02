using AutoFlow.Application.Services;
using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Interfaces.Services
{
    public interface ITokenService
    {
        TokenGerado GerarToken(Usuario usuario);
    }
}
