using  Ecommerce.Services.DAO.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Ecommerce.Services.DAO.Interfaces.IRepository
{
    public interface IUserRepository
    {
        Task<UserDTO> GetUserWithDetailsAsync(int id);
        Task<IEnumerable<UserDTO>> GetAllUsersAsDTOAsync();
        Task CreateUserFromDTOAsync(UserDTO userDto);
        Task DeleteUserFromDTOAsync(int id);
        Task UpdateNewPasswordAsync(UserDTO userDto);
    }
}
