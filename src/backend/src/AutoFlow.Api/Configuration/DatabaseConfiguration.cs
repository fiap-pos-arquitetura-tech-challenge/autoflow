using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Models;
using AutoFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoFlow.Api.Configuration
{
    public static class DatabaseConfiguration
    {
        private const string SenhaDefaultDesenvolvimento = "DevOnly@123456";

        public static async Task ApplyMigrationsAndSeedAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(nameof(DatabaseConfiguration));

            await dbContext.Database.MigrateAsync();
            await SeedColaboradorInicialAsync(dbContext, configuration, logger);
        }

        private static async Task SeedColaboradorInicialAsync(
            AppDbContext dbContext,
            IConfiguration configuration,
            ILogger logger)
        {
            if (await dbContext.Set<Usuario>().AnyAsync(x => x.Perfil == Perfil.Colaborador))
                return;

            var nome = configuration["Bootstrap:Colaborador:Nome"] ?? "Administrador AutoFlow";
            var email = configuration["Bootstrap:Colaborador:Email"];
            var senha = configuration["Bootstrap:Colaborador:Senha"];

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(senha))
                return;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                throw new InvalidOperationException("Bootstrap do colaborador inicial exige Bootstrap:Colaborador:Email e Bootstrap:Colaborador:Senha.");

            if (senha == SenhaDefaultDesenvolvimento)
                logger.LogWarning("Usando senha default de desenvolvimento para o colaborador inicial. Defina AUTOFLOW_BOOTSTRAP_SENHA para sobrescrever.");

            dbContext.Set<Usuario>().Add(new Usuario(nome, email, senha, Perfil.Colaborador));
            await dbContext.SaveChangesAsync();
        }
    }
}
