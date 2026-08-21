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

    await app.Services.ApplyMigrationsAndSeedAsync();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseSecurityConfiguration();

app.MapAuthEndpoints();
app.MapClienteEndpoints();
app.MapServicoEndpoints();
app.MapUsuarioEndpoints();
app.MapVeiculoEndpoints();

app.MapPecaInsumoEndpoints();

app.MapEstoqueEndpoints();

app.Run();

public partial class Program
{
}
