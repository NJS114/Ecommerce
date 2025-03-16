using  Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Ecommerce.Services.DAO.Interfaces.IRepository
{
    public interface IUserRepository
    {
        Task<UserDTO> GetUserWithDetailsAsync(string email);

        Task<IEnumerable<UserDTO>> GetAllUsersAsDTOAsync(int page, int pageSize);
        Task RegisterFromDTOAsync(RegisterDTO userDto);
        Task CreateUserDTOFromDTOAsync(UserDTO userDto);
        Task DeleteUserFromDTOAsync(string id);
        Task UpdateNewPasswordAsync(RegisterDTO userDto);
        Task<IActionResult> UpdateUserDTOAsync(string id, UserDTO userDto);
        //Task CreateMarchantFromDTOAsync(MerchantRegisterDTO merchantDto);
        Task<UserDTO> GetUserByIdAsync(string id);
    }
}
