using Ecommerce.Services.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IUserReadInterface
    {
        Task<User> GetUserByEmailAsync(string Email);
        Task<User> GetUserByIdAsync(string id);
        Task<IEnumerable<User>> GetAllUserAsync(int page, int pageSize);

    }
}
