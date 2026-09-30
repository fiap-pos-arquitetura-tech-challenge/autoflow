using AutoFlow.Api.Configuration;
using AutoFlow.Api.Endpoints;
using AutoFlow.Api.Middlewares;
using AutoFlow.Api.OpenApi;
using AutoFlow.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ResolveDependencies();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSecurityConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();    
}

var applyMigrations = app.Configuration.GetValue<bool>("Database:ApplyMigrations");

if (app.Environment.IsDevelopment() || applyMigrations)
{
    await app.Services.ApplyMigrationsAndSeedAsync();
}

if (applyMigrations)
{
    return;
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseSecurityConfiguration();


app.MapGet("/", () => "Ok System is running, ateração feita!");
app.MapAuthEndpoints();
app.MapClienteEndpoints();
app.MapServicoEndpoints();
app.MapUsuarioEndpoints();
app.MapVeiculoEndpoints();

app.MapPecaInsumoEndpoints();

app.MapEstoqueEndpoints();
app.MapOrdemServicoEndpoints();

app.Run();

public partial class Program
{
}
