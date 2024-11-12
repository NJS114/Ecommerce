using  Ecommerce.Services.DAO.DTOs;
using  Ecommerce.Services.DAO.Interfaces.Services;
using  Ecommerce.Services.DAO.Interfaces.UserInterface;
using  Ecommerce.Services.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Ecommerce.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase, IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FullName.Split(' ')[0],
                LastName = model.FullName.Split(' ').Last()
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest(new { message = "Role name is required." });
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return Ok(new { message = "Role created successfully" });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost("addUserToRole")]
        public async Task<IActionResult> AddUserToRole([FromBody] UserRoleDTO userRole)
        {
            var user = await _userManager.FindByIdAsync(userRole.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var result = await _userManager.AddToRoleAsync(user, userRole.RoleName);
            if (result.Succeeded)
            {
                return Ok(new { message = "User added to role successfully" });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost("removeUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole([FromBody] UserRoleDTO userRole)
        {
            var user = await _userManager.FindByIdAsync(userRole.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, userRole.RoleName);
            if (result.Succeeded)
            {
                return Ok(new { message = "User removed from role successfully" });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList(); 
            return Ok(roles);
        }
    }

    public class UserRoleDTO
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }
}
