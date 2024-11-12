using  Ecommerce.Services.DAO.Builders;
using  Ecommerce.Services.DAO.DTOs;
using  Ecommerce.Services.DAO.Interfaces.IRepository;
using  Ecommerce.Services.DAO.Interfaces.UserInterface;
using  Ecommerce.Services.DAO.Mapping;
using  Ecommerce.Services.DAO.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Ecommerce.Services.DAO.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IUserDAO _userDao;

        public UserRepository(IUserDAO userDao)
        {
            _userDao = userDao;
        }

        public async Task<UserDTO> GetUserWithDetailsAsync(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            return UserMapping.ToDTO(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsDTOAsync()
        {
            var users = await _userDao.GetAllUserAsync();
            return UserMapping.ToDTOList(users);
        }

        public async Task CreateUserFromDTOAsync(UserDTO userDto)
        {
            var user = new UserBuilder(new User())
                .SetName(userDto.FullName)
                .SetEmail(userDto.Email)
                .SetPassword(userDto.Password)
                .Build();

            await _userDao.CreateUserAsync(user);
        }

        public async Task DeleteUserFromDTOAsync(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user != null)
            {
                await _userDao.DeleteUserAsync(id);
            }
        }

        public async Task UpdateNewPasswordAsync(UserDTO userDto)
        {
            if (userDto == null || string.IsNullOrWhiteSpace(userDto.Password))
            {
                throw new ArgumentNullException(nameof(userDto), "L'utilisateur ou le mot de passe ne peut pas être nul ou vide.");
            }

            var user = new User
            {
                Id = userDto.Id,
                Password = userDto.Password
            };

            await _userDao.UpdatePasswordAsync(user);
        }
    }
}
