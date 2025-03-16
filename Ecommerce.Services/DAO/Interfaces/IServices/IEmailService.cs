namespace Ecommerce.Services.DAO.Interfaces.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
