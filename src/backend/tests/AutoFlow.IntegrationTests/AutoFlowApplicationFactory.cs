using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Models;
using AutoFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutoFlow.IntegrationTests;
public class AutoFlowApplicationFactory : WebApplicationFactory<Program>
{
    public const string ColaboradorEmail = "colaborador.teste@autoflow.com";
    public const string ColaboradorSenha = "TesteIntegracao@123";
    public const int ClienteId = 1;
    public const string ClienteDocumento = "11144477735";
    public const string ClienteEmail = "cliente.teste@autoflow.com";
    public const string ClienteSenha = "ClienteTeste@123";
    public const int OutroClienteId = 2;
    public const string OutroClienteEmail = "outro.cliente@autoflow.com";
    public const string OutroClienteSenha = "OutroCliente@123";

    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Program.cs lê Jwt:Secret direto de builder.Configuration antes do Build(),
        // então precisa estar disponível como variável de ambiente antes do host ser criado
        // (ConfigureAppConfiguration/ConfigureServices só se aplicam depois desse ponto).
        Environment.SetEnvironmentVariable("Jwt__Secret", "TestingOnlySecretKey_MustBeAtLeast32BytesLong!");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "AutoFlow.Api.Testing");
        Environment.SetEnvironmentVariable("Jwt__Audience", "AutoFlow.Client.Testing");

        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            _connection = new SqliteConnection("DataSource=:memory:");

            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            using var scope = services
                .BuildServiceProvider()
                .CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            // Program.cs só semeia o colaborador inicial em ambiente Development;
            // em Testing precisamos criar o usuário base aqui para poder logar nos testes.
            db.Set<Usuario>().Add(new Usuario(
                "Colaborador Teste",
                ColaboradorEmail,
                ColaboradorSenha,
                Perfil.Colaborador));

            var cliente = new Cliente(
                "Cliente Teste",
                ClienteDocumento,
                "11999999999",
                ClienteEmail);

            var outroCliente = new Cliente(
                "Outro Cliente",
                "52998224725",
                "11988888888",
                OutroClienteEmail);

            db.Set<Cliente>().AddRange(cliente, outroCliente);
            db.SaveChanges();

            db.Set<Usuario>().AddRange(
                new Usuario(
                    "Cliente Teste",
                    ClienteEmail,
                    ClienteSenha,
                    Perfil.Cliente,
                    cliente.Id),
                new Usuario(
                    "Outro Cliente",
                    OutroClienteEmail,
                    OutroClienteSenha,
                    Perfil.Cliente,
                    outroCliente.Id));

            db.SaveChanges();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _connection?.Dispose();
    }
}
