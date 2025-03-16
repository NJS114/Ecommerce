namespace Ecommerce.Services.DAO.Interfaces.IServices
{
    public interface INewsletterService
    {
        Task SendNewsletterToAllUsersAsync(string subject, string body, int page = 1, int pageSize = 100);
    }
}
