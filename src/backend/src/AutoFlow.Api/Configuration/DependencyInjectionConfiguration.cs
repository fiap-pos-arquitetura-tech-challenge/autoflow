using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Services;
using AutoFlow.Infrastructure.Persistence.Repositories;
using AutoFlow.Infrastructure.Security;

namespace AutoFlow.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
            services.AddScoped<IVeiculoService, VeiculoService>();
            services.AddScoped<IVeiculoRepositorio, VeiculoRepositorio>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IServicoService, ServicoService>();
            services.AddScoped<IServicoRepositorio, ServicoRepositorio>();

            return services;
        }
    }
}
