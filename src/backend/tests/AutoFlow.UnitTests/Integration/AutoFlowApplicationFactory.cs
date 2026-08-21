using AutoFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutoFlow.UnitTests.Integration;
public class AutoFlowApplicationFactory : WebApplicationFactory<Program>
{
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
            services.RemoveAll(typeof(AppDbContext));
            
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

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
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _connection?.Dispose();
    }
}
