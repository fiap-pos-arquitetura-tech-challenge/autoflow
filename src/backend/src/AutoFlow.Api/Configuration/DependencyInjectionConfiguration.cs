using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services;
using AutoFlow.Infrastructure.Persistence.Repositories;

namespace AutoFlow.Api.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection ResolveDependencies(
        this IServiceCollection services)
    {
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IClienteRepositorio, ClienteRepositorio>();

        services.AddScoped<IPecaInsumoService, PecaInsumoService>();
        services.AddScoped<IPecaInsumoRepositorio, PecaInsumoRepositorio>();

        services.AddScoped<IEstoqueService, EstoqueService>();
        services.AddScoped<IEstoqueRepositorio, EstoqueRepositorio>();

        return services;
    }
}