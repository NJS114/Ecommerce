
    using Ecommerce.Services.DAO.DTOs;
    using Ecommerce.Services.DAO.Interfaces.IRepository;
using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

using System.Threading.Tasks;

    namespace  Ecommerce.Services.Controllers
    {
        [Authorize(Roles = "Admin")]
        [Route("api/[controller]")]
        [ApiController]
        public class UserController : ControllerBase
        {
            private readonly IUserRepository _userRepository;

            public UserController(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            // GET: api/users
            [HttpGet]
            public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
            {
                var users = await _userRepository.GetAllUsersAsDTOAsync();
                return Ok(users);
            }

            // GET: api/users/{id}
            [HttpGet("{id}")]
            public async Task<ActionResult<UserDTO>> GetUser(int id)
            {
                var user = await _userRepository.GetUserWithDetailsAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult> CreateUserAsync([FromBody] UserDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest("L'utilisateur ne peut pas être nul.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }   
            if (string.IsNullOrWhiteSpace(userDto.FullName) ||
                string.IsNullOrWhiteSpace(userDto.Email) ||
                string.IsNullOrWhiteSpace(userDto.Password))
            {
                return BadRequest("Tous les champs sont requis.");
            }

            // Assurez-vous que l'email est valide
            if (!IsValidEmail(userDto.Email))
            {
                return BadRequest("L'adresse email est invalide.");
            }

            await _userRepository.CreateUserFromDTOAsync(userDto);
            return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
            public async Task<ActionResult> UpdateUser(int id, [FromBody] UserDTO userDto)
            {
                if (userDto == null || userDto.Id != id)
                {
                    return BadRequest("Données de l'utilisateur invalides.");
                }

                await _userRepository.UpdateNewPasswordAsync(userDto);
                return NoContent();
            }

            // DELETE: api/users/{id}
            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                await _userRepository.DeleteUserFromDTOAsync(id); 
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Utilisateur avec ID {id} introuvable.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        }
    }


