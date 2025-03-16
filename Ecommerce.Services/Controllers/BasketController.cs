using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Interfaces;
using Ecommerce.Services.DAO.Interfaces.IServices;
using Ecommerce.Services.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ecommerce.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet("get-basket/{userId}")]
        public async Task<IActionResult> GetBasketByUserIdAsync(string userId)
        {
            var basket = await _basketService.GetBasketByUserIdAsync(userId);
            if (basket == null)
            {
                return NotFound();
            }
            return Ok(basket);
        }

        [HttpPost("add-to-basket/{userId}")]
        public async Task<IActionResult> AddToBasketAsync(string userId, [FromBody] BasketItem basketItem)
        {
            await _basketService.AddToBasketAsync(userId, basketItem);
            return Ok(new { message = "Article ajouté au panier" });
        }

        [HttpPost("remove-from-basket/{userId}/{bookId}")]
        public async Task<IActionResult> RemoveFromBasketAsync(string userId, string bookId)
        {
            await _basketService.RemoveFromBasketAsync(userId, bookId);
            return Ok(new { message = "Article supprimé du panier" });
        }

        [HttpPost("update-item-quantity/{userId}/{bookId}")]
        public async Task<IActionResult> UpdateBasketItemQuantityAsync(string userId, string bookId, [FromBody] int newQuantity)
        {
            await _basketService.UpdateBasketItemQuantityAsync(userId, bookId, newQuantity);
            return Ok(new { message = "Quantité de l'article mise à jour" });
        }

        [HttpPost("empty-basket/{userId}")]
        public async Task<IActionResult> EmptyBasketAsync(string userId)
        {
            await _basketService.EmptyBasketAsync(userId);
            return Ok(new { message = "Panier vidé" });
        }

        [HttpPost("confirm-purchase/{userId}")]
        public async Task<IActionResult> ConfirmPurchaseAsync(string userId)
        {
            await _basketService.ConfirmPurchaseAsync(userId);
            return Ok(new { message = "Achat confirmé et statut mis à jour" });
        }

        [HttpPost("update-item-status/{userId}/{bookId}")]
        public async Task<IActionResult> UpdateItemStatusAsync(string userId, string bookId, [FromBody] BasketItemStatus newStatus)
        {
            await _basketService.UpdateItemStatusAsync(userId, bookId, newStatus);
            return Ok(new { message = "Statut de l'article mis à jour" });
        }

        [HttpPost("apply-discount/{userId}")]
        public async Task<IActionResult> ApplyDiscountAfterPurchaseAsync(string userId)
        {
            await _basketService.ApplyDiscountAfterPurchaseAsync(userId);
            return Ok(new { message = "Remise appliquée" });
        }
        [HttpPut("update-item-quantity/{userId}/{productId}")]
        public async Task<IActionResult> UpdateItemQuantityAsync(string userId, string productId, [FromBody] int newQuantity)
        {
            if (newQuantity <= 0)
            {
                return BadRequest(new { message = "La quantité doit être supérieure à zéro." });
            }

            try
            {
                // Appeler le service pour mettre à jour la quantité de l'article
                await _basketService.UpdateBasketItemQuantityAsync(userId, productId, newQuantity);
                return Ok(new { message = "Quantité mise à jour avec succès." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors de la mise à jour de la quantité.", details = ex.Message });
            }
        }

    }
}
