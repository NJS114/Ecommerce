using Ecommerce.Services.DAO.Interfaces.IServices;
using Ecommerce.Services.DAO.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Ecommerce.Services.DAO.Interfaces.IRepository;

namespace Ecommerce.Services.Services
{
    public class NewsletterService : INewsletterService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public NewsletterService(IEmailService emailService, IUserRepository userRepository, IConfiguration configuration)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task SendNewsletterToAllUsersAsync(string subject, string body, int page = 1, int pageSize = 100)
        {
            var users = await _userRepository.GetAllUsersAsDTOAsync(page, pageSize);

            var emails = users.Select(user => user.Email).ToList();

            foreach (var email in emails)
            {
                await _emailService.SendEmailAsync(email, subject, body);
            }
        }
    }
}
