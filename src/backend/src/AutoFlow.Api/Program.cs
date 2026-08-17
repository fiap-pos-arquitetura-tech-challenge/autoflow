using AutoFlow.Api.Configuration;
using AutoFlow.Api.Endpoints;
using AutoFlow.Api.Middlewares;
using AutoFlow.Infrastructure;
using AutoFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ResolveDependencies();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapClienteEndpoints();
app.MapVeiculoEndpoints();

app.Run();

public partial class Program
{
}
