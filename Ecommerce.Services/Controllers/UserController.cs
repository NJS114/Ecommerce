using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace  Ecommerce.Services.Controllers
{
    [AllowAnonymous]

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public UserController(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        // GET: api/users
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegisterDTO>>> GetAllUsers(int page, int pageSize)
        {
            var users = await _userRepository.GetAllUsersAsDTOAsync( page,  pageSize);
            return Ok(users);
        }

        // GET: api/users/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<RegisterDTO>> GetUser(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/users
        [AllowAnonymous]
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

            if (string.IsNullOrWhiteSpace(userDto.FirstName) ||
                string.IsNullOrWhiteSpace(userDto.Email) ||
                string.IsNullOrWhiteSpace(userDto.LastName) ||
                string.IsNullOrWhiteSpace(userDto.Password))
            {
                return BadRequest("Tous les champs sont requis.");
            }

            if (!IsValidEmail(userDto.Email))
            {
                return BadRequest("L'adresse email est invalide.");
            }

            await _userRepository.CreateUserDTOFromDTOAsync(userDto);

           
            var token = _jwtService.GenerateToken(userDto.Email);

            return Ok(new
            {
                Message = "Utilisateur créé avec succès.",
                Token = token
            });
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
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UserDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Les données de l'utilisateur sont invalides.");
            }

            // Comparer les deux IDs (id et userDto.Id) en tant que chaînes
            if (id != userDto.Id)
            {
                return BadRequest("L'ID de l'utilisateur ne correspond pas à l'ID dans l'URL.");
            }

            await _userRepository.UpdateUserDTOAsync(id, userDto);
            return NoContent();
        }


        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
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


