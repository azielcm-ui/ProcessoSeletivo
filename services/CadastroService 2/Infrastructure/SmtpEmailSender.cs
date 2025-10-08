using System.Threading.Tasks;
using System;

namespace Cadastro.Infrastructure
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class SmtpEmailSender : IEmailSender
    {
        // Placeholder implementation using MailHog via SMTP in compose
        public Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine($"[EmailSender] To={to} Subject={subject}\n{body}");
            return Task.CompletedTask;
        }
    }
}
