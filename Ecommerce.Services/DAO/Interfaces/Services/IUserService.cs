using  Ecommerce.Services.Controllers;
using  Ecommerce.Services.DAO.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace  Ecommerce.Services.DAO.Interfaces.Services
{
    public interface IUserService
    {
        Task<IActionResult> Register(UserDTO model);
        Task<IActionResult> CreateRole(string roleName);
        Task<IActionResult> AddUserToRole(UserRoleDTO userRole);
        Task<IActionResult> RemoveUserFromRole(UserRoleDTO userRole);
        IActionResult GetAllRoles();
    }
}
