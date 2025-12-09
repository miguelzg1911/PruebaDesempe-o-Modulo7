namespace TalentoPlus.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent);
}
