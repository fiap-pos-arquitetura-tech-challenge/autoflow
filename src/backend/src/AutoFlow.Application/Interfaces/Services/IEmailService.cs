namespace AutoFlow.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task EnviarAsync(string destinatario, string assunto, string corpo);
    }
}
