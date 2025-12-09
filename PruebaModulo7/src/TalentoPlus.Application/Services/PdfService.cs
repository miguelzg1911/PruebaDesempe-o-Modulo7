using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class PdfService : IPdfService
{
    public Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent)
    {
        return Task.FromResult(new byte[] { 0x00 });
    }
}
