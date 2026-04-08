using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using BoletoVerifier.Models;

namespace BoletoVerifier.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarAsync(string emailDestino, BoletoInfo info)
    {
        var mensagem = new MimeMessage();
        mensagem.From.Add(MailboxAddress.Parse(_config["Email:Remetente"]));
        mensagem.To.Add(MailboxAddress.Parse(emailDestino));
        mensagem.Subject = "Informações do Boleto";

        mensagem.Body = new TextPart("plain")
        {
            Text = $"""
                Boleto processado com sucesso!

                Banco:      {info.Banco}
                Valor:      R$ {info.Valor:N2}
                Vencimento: {info.Vencimento}

                Código original:
                {info.CodigoOriginal}
                """
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["Email:Remetente"], _config["Email:Senha"]);
        await smtp.SendAsync(mensagem);
        await smtp.DisconnectAsync(true);
    }
}