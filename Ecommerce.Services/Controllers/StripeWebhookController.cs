using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Ecommerce.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeWebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> HandleStripeEvent()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], "your-webhook-secret");

                if (stripeEvent.Type == "invoice.payment_succeeded")
                {
                }
                else if (stripeEvent.Type == "invoice.payment_failed")
                {
                }
                else if (stripeEvent.Type == "customer.subscription.updated")
                {
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }
        }
    }
}
