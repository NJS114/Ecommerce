using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Ecommerce.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("create/{userId}")]
        public async Task<IActionResult> CreateSubscription(string userId, [FromBody] CreateSubscriptionDTO subscriptionDTO)
        {
            var response = await _subscriptionService.CreateSubscriptionAsync(userId, subscriptionDTO.PriceId, subscriptionDTO.PaymentMethodId);
            if (response.Status == PaymentStatus.Completed)
            {
                return Ok(response);
            }
            return BadRequest(response.ErrorMessage);
        }

        [HttpPost("cancel/{subscriptionId}")]
        public async Task<IActionResult> CancelSubscription(string subscriptionId)
        {
            var response = await _subscriptionService.CancelSubscriptionAsync(subscriptionId);
            if (response.Status == PaymentStatus.Completed)
            {
                return Ok(response);
            }
            return BadRequest(response.ErrorMessage);
        }

        [HttpPut("update/{subscriptionId}")]
        public async Task<IActionResult> UpdateSubscription(string subscriptionId, [FromBody] UpdateSubscriptionDTO subscriptionDTO)
        {
            var response = await _subscriptionService.UpdateSubscriptionPlanAsync(subscriptionId, subscriptionDTO.NewPriceId);
            if (response.Status == PaymentStatus.Completed)
            {
                return Ok(response);
            }
            return BadRequest(response.ErrorMessage);
        }
    }
}
