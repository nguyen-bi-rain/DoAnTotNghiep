using AuthJWT.Domain.Contracts;

namespace AuthJWT.Services.Interfaces
{
    public interface ISendEmailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendEmailWithTemplateAsync(MailRequest mailRequest, string templatePath);
    }
}