using System.Text;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services;
using AutoFlow.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AutoFlow.Infrastructure.Security
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        public TokenGerado GerarToken(Usuario usuario)
        {
            var secret = configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("Jwt:Secret não configurado.");

            var expiresInMinutes = int.TryParse(configuration["Jwt:ExpiresInMinutes"], out var minutos) ? minutos : 60;
            var expiraEm = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            var claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = usuario.Id.ToString(),
                [JwtRegisteredClaimNames.Email] = usuario.Email.Endereco,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                [System.Security.Claims.ClaimTypes.Name] = usuario.Nome,
                [System.Security.Claims.ClaimTypes.Role] = usuario.Perfil.ToString()
            };

            if (usuario.ClienteId is not null)
                claims["clienteId"] = usuario.ClienteId.Value.ToString();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                Claims = claims,
                Expires = expiraEm,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);

            return new TokenGerado(token, expiraEm);
        }
    }
}
