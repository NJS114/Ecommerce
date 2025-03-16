using Ecommerce.Services.DAO.Models;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Services.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IOrderRepository _orderRepository;
        public OrderController(OrderService orderService, IOrderRepository orderRepository)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository)); 
        }

        #region CRUD Methods

        // Crée une commande à partir du panier de l'utilisateur
        [HttpPost("create-order/{userId}")]
        public async Task<IActionResult> CreateOrder(string userId)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(userId);

                if (order.Status == OrderStatus.Completed)
                {
                    return Ok(order);  // La commande a été créée avec succès et le paiement a réussi.
                }
                else
                {
                    return BadRequest("Échec du paiement ou de la création de la commande.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la création de la commande : {ex.Message}");
            }
        }

        // Récupère une commande par son ID
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("L'ID de la commande ne peut pas être nul ou vide.");
            }

            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound("Commande non trouvée.");
            }

            return Ok(order);
        }

        // Récupère toutes les commandes d'un utilisateur par son ID
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("L'ID de l'utilisateur ne peut pas être nul ou vide.");
            }

            var orders = await _orderRepository.GetOrdersByUserId(userId);
            if (orders == null || orders.Count == 0)
            {
                return NotFound("Aucune commande trouvée pour cet utilisateur.");
            }

            return Ok(orders);
        }

        // Met à jour le statut d'une commande
        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] OrderStatus newStatus)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("L'ID de la commande ne peut pas être nul ou vide.");
            }

            var updatedOrder = await _orderRepository.UpdateOrderStatus(orderId, newStatus);
            if (updatedOrder == null)
            {
                return NotFound("Commande non trouvée.");
            }

            return Ok(updatedOrder);
        }

        // Supprime une commande
        [HttpDelete("delete-order/{orderId}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("L'ID de la commande ne peut pas être nul ou vide.");
            }

            var deletedOrder = await _orderRepository.DeleteOrder(orderId);
            if (deletedOrder == null)
            {
                return NotFound("Commande non trouvée.");
            }

            return Ok(deletedOrder);
        }

        // Récupère toutes les commandes dans une période spécifiée
        [HttpGet("all-orders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var orders = await _orderRepository.GetAllOrders(startDate, endDate);
                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
