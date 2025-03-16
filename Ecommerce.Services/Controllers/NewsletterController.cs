using Ecommerce.Services.DAO.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterController : Controller
    {
        private readonly INewsletterService _newsletterService;
        private readonly IEmailService _emailService;


        public NewsletterController(INewsletterService newsletterService, IEmailService emailService)
        {
            _newsletterService = newsletterService;
            _emailService = emailService;
        }
        [HttpPost("newsletter")]
        public async Task<IActionResult> SendNewsletter(string subject, string body)
        {

            // Envoie la newsletter à tous les utilisateurs
            await _newsletterService.SendNewsletterToAllUsersAsync(subject, body);

            return Ok("Newsletter envoyée avec succès à tous les utilisateurs.");
        }
        [HttpPost("sendmessage")]
        public async Task<IActionResult> SendNewsletterToUser(string email, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("L'email est requis.");

            await _emailService.SendEmailAsync(email, subject, body);
            return Ok("Newsletter envoyée avec succès.");
        }
    }
}
