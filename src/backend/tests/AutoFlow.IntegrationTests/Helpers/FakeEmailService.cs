using AutoFlow.Application.Interfaces.Services;

namespace AutoFlow.IntegrationTests.Helpers;

public class FakeEmailService : IEmailService
{
    public Task EnviarAsync(string destinatario, string assunto, string corpo) => Task.CompletedTask;
}
