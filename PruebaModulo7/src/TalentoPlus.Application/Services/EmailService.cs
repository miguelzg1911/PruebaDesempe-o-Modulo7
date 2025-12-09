using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"Email to {to}: {subject}");
        return Task.CompletedTask;
    }
}
