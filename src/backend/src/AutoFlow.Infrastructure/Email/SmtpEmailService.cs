using AutoFlow.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace AutoFlow.Infrastructure.Email
{
    public class SmtpEmailService(IConfiguration configuration) : IEmailService
    {
        public async Task EnviarAsync(string destinatario, string assunto, string corpo)
        {
            var remetente = configuration["Smtp:From"] ?? "naoresponda@autoflow.local";
            var host = configuration["Smtp:Host"] ?? "localhost";
            var porta = int.TryParse(configuration["Smtp:Port"], out var portaConfigurada) ? portaConfigurada : 1025;
            var usarSsl = bool.TryParse(configuration["Smtp:UseSsl"], out var ssl) && ssl;
            var usuario = configuration["Smtp:User"];
            var senha = configuration["Smtp:Password"];

            var mensagem = new MimeMessage();
            mensagem.From.Add(MailboxAddress.Parse(remetente));
            mensagem.To.Add(MailboxAddress.Parse(destinatario));
            mensagem.Subject = assunto;
            mensagem.Body = new TextPart("plain") { Text = corpo };

            using var client = new SmtpClient();

            await client.ConnectAsync(host, porta, usarSsl);

            if (!string.IsNullOrWhiteSpace(usuario))
                await client.AuthenticateAsync(usuario, senha);

            await client.SendAsync(mensagem);
            await client.DisconnectAsync(true);
        }
    }
}
