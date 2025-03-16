using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net.Mail;
using System.Net;
using Ecommerce.Services.DAO.Interfaces.IRepository;

namespace Ecommerce.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IMongoDbConnection _mongoDbConnection;

        public HealthController(IMongoDbConnection mongoDbConnection)
        {
            _mongoDbConnection = mongoDbConnection;
        }

        [HttpGet]
        public IActionResult GetHealth()
        {
            // Vérifications
            var mongoStatus = CheckMongoDb();
            var stripeStatus = CheckStripe();
            var smtpStatus = CheckSmtp();
            var jwtStatus = CheckJwt();

            /*_logger.LogInformation("Health Check Results: MongoDB={MongoStatus}, Redis={RedisStatus}, Stripe={StripeStatus}, SMTP={SmtpStatus}, JWT={JwtStatus}",
           mongoStatus,  stripeStatus, smtpStatus, jwtStatus);*/

            return Ok(new
            {
                Status = "Healthy",
                Services = new[]
                {
                new { Name = "MongoDB", Status = mongoStatus },
                new { Name = "Stripe", Status = stripeStatus },
                new { Name = "SMTP", Status = smtpStatus },
                new { Name = "JWT", Status = jwtStatus }
            }
            });
        }

        private string CheckMongoDb()
        {
            try
            {
                var database = _mongoDbConnection.GetDatabase();
                return database != null ? "Operational" : "Not operational";
            }
            catch
            {
                return "Not operational";
            }
        }

       

        private string CheckStripe()
        {
            try
            {
                var service = new Stripe.BalanceService();
                return service.Get() != null ? "Operational" : "Not operational";
            }
            catch
            {
                return "Not operational";
            }
        }

        private string CheckSmtp()
        {
            try
            {
                using var client = new SmtpClient("smtp.server.com", 587)
                {
                    Credentials = new NetworkCredential("username", "password"),
                    EnableSsl = true,
                };
                client.Send(new MailMessage("from@example.com", "to@example.com", "Health Check", "SMTP is operational."));
                return "Operational";
            }
            catch
            {
                return "Not operational";
            }
        }

        private string CheckJwt()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_KEY"))
                ? "Operational"
                : "Not operational";
        }
    }

}
